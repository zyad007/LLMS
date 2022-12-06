using LLS.BLL.Configs;
using LLS.BLL.IServices;
using LLS.Common.Dto.Logins;
using LLS.Common.Models;
using LLS.Common.Transfere_Layer_Object;
using LLS.DAL.Data;
using LLS.DAL.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace LLS.BLL.Services
{
    public class AccountService : IAccountService
    {
        private readonly JwtConfig _jwtConfig;
        private readonly TokenValidationParameters _tokenValidationParameters;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly AppDbContext _context;
        public AccountService(TokenValidationParameters tokenValidationParameters,
                              IOptionsMonitor<JwtConfig> optionsMonitor,
                              UserManager<IdentityUser> userManager,
                              RoleManager<IdentityRole> roleManager,
                              AppDbContext cotext)
        {
            _tokenValidationParameters = tokenValidationParameters;
            _jwtConfig = optionsMonitor.CurrentValue;
            _userManager = userManager;
            _roleManager = roleManager;
            _context = cotext;
        }

        public async Task<AuthResult> Login(Login login)
        {
            var userExist = await _userManager.FindByEmailAsync(login.Email);

            if (userExist == null)
            {
                return new AuthResult
                {
                    Status = false,
                    Error = new List<string>()
                    {
                        "Something wrong with Email or the Password"
                    }
                };
            }

            //check if user have valid password
            var isCorrect = await _userManager.CheckPasswordAsync(userExist, login.Password);

            if (isCorrect)
            {
                var user = await _context.Users.FirstOrDefaultAsync(x => x.Email.ToLower() == login.Email.ToLower());
                //Generate Token
                var jwtToken = await GenerateJwtToken(userExist);
                jwtToken.Role = user.Role;
                jwtToken.Email = user.Email;
                jwtToken.Idd = user.Idd;
                return jwtToken;
            }
            else
            {
                //Wrong password
                return new AuthResult
                {
                    Status = false,
                    Error = new List<string>()
                    {
                        "Something wrong with Email or the Password"
                    }
                };
            }
        }

        public async Task<AuthResult> RefreshToken(TokenRequest tokenRequest)
        {
            var result = await VerifyAndGenerateToken(tokenRequest);

            if (result == null)
            {
                return new AuthResult()
                {
                    Error = new List<string>() {
                    "Invalid tokens"
                },
                    Status = false
                };
            }
            return result;
        }

        //
        public class TokenRequest
        {
            [Required]
            public string Token { get; set; }
            [Required]
            public string RefreshToken { get; set; }
        }

        private async Task<AuthResult> VerifyAndGenerateToken(TokenRequest tokenRequest)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var storedToken = await _context.RefreshTokens.FirstOrDefaultAsync(x => x.Token == tokenRequest.RefreshToken);

            // validation 1 - validate existence of the token
            if (storedToken == null)
            {
                return new AuthResult()
                {
                    Status = false,
                    Error = "Token does not exist"
                };
            }

            // Validation 2 - validate if used
            if (storedToken.IsUsed)
            {
                return new AuthResult()
                {
                    Status = false,
                    Error = "Token has been used"
                };
            }

            // Validation 3 - validate if revoked
            if (storedToken.IsRevorked)
            {
                return new AuthResult()
                {
                    Status = false,
                    Error = "Token has been revoked"
                        
                };
            }

            try
            {
                // Validation 4 - Validation JWT token format
                var tokenInVerification = jwtTokenHandler.ValidateToken(tokenRequest.Token, _tokenValidationParameters, out var validatedToken);

                // Validation 5 - Validate encryption alg
                if (validatedToken is JwtSecurityToken jwtSecurityToken)
                {
                    var result = jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase);

                    if (result == false)
                    {
                        return null;
                    }
                }

                // Validation 6 - validate expiry date
                var utcExpiryDate = long.Parse(tokenInVerification.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Exp).Value);

                var expiryDate = UnixTimeStampToDateTime(utcExpiryDate);

                if (expiryDate > DateTime.UtcNow)
                {
                    return new AuthResult()
                    {
                        Status = false,
                        Error = "Token has not yet expired"
                    };
                }


                // Validation 7 - validate the id
                var jti = tokenInVerification.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti).Value;

                if (storedToken.JwtId != jti)
                {
                    return new AuthResult()
                    {
                        Status = false,
                        Error = "Token doesn't match"
                    };
                }

                return null;
            }
            catch (SecurityTokenExpiredException)
            {
                // update current token 
                storedToken.IsUsed = true;
                _context.RefreshTokens.Update(storedToken);
                await _context.SaveChangesAsync();

                // Generate a new token
                var dbUser = await _userManager.FindByIdAsync(storedToken.UserId);
                return await GenerateJwtToken(dbUser);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Unable to decode the payload") ||
                    ex.Message.Contains("Signature validation failed") ||
                    ex.Message.Contains("Unable to decode the header"))
                {
                    return new AuthResult()
                    {
                        Status = false,
                        Error = new List<string>() {
                            "Some thing went wrong"
                        }
                    };
                }

                return new AuthResult()
                {
                    Status = false,
                    Error = new List<string>() {
                            ex.Message
                        }
                };
            }
        }


        private static DateTime UnixTimeStampToDateTime(long unixTimeStamp)
        {
            var dateTimeVal = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTimeVal = dateTimeVal.AddSeconds(unixTimeStamp).ToUniversalTime();

            return dateTimeVal;
        }

        private async Task<AuthResult> GenerateJwtToken(IdentityUser user)
        {
            var jwtHandler = new JwtSecurityTokenHandler();

            var key = Encoding.ASCII.GetBytes(_jwtConfig.Secret);

            var claims = await GetValidClaims(user);

            var token = new JwtSecurityToken(
                expires: DateTime.UtcNow.AddHours(10),
                claims: claims,
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256));

            //Convert to string
            var jwtToken = jwtHandler.WriteToken(token);

            var refreshToken = new RefreshToken()
            {
                JwtId = token.Id,
                IsRevorked = false,
                IsUsed = false,
                UserId = user.Id,
                AddedDate = DateTime.UtcNow,
                ExpirayDate = DateTime.UtcNow.AddMonths(6),
                Token = RandomString(35) + Guid.NewGuid()
            };

            await _context.RefreshTokens.AddAsync(refreshToken);
            await _context.SaveChangesAsync();

            return new AuthResult()
            {
                Token = jwtToken,
                Status = true,
                RefreshToken = refreshToken.Token
            };
        }

        private async Task<List<Claim>> GetValidClaims(IdentityUser user)
        {
            var options = new IdentityOptions();

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // id for Token used for refresh token
            };

            // Getting the claims that we have assigned to user
            //await _userManager.AddClaimAsync(user,new Claim("AddDeleteEdit_Course", "AddDeleteEdit_Course"));
            //var userClaims = await _userManager.GetClaimsAsync(user);
            //claims.AddRange(userClaims);


            var userRoles = await _userManager.GetRolesAsync(user);

            if (userRoles.Any())
            {
                foreach (var userRole in userRoles)
                {
                    var _role = await _roleManager.FindByNameAsync(userRole);
                    if (_role != null)
                    {
                        claims.Add(new Claim("Roles", userRole));

                        var roleClaims = await _roleManager.GetClaimsAsync(_role);
                        claims.AddRange(roleClaims);
                    }
                }
            }
            return claims;
        }

        private static string RandomString(int lenght)
        {
            var random = new Random();
            var chars = "ABCDEFGHIJLKMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, lenght)
                .Select(x => x[random.Next(x.Length)]).ToArray());
        }
    }
}
