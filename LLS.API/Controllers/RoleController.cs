using LLS.Common.Dto;
using LLS.Common.Transfere_Layer_Object;
using LLS.DAL.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Reflection;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using LLS.DAL.Data;
using LLS.Common.Enums;
using Microsoft.AspNetCore.Authorization;
using AutoMapper;
using LLS.Common.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace LLS.API.Controllers
{
    public class RoleController : BaseController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        public RoleController(IUnitOfWork unitOfWork, 
            RoleManager<IdentityRole> roleManager,
             AppDbContext context,
             IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _roleManager = roleManager;
            _context = context;
            _mapper = mapper;
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("Get-Info")]
        public async Task<IActionResult> GetUserInfo()
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            if(userId == null)
            {
                return BadRequest(new Result()
                {
                    Message = "Invalid Payload",
                    Status = false
                });
            }

            var user = _context.Users.FirstOrDefault(x => x.IdentityId.ToString() == userId);

            if (user == null)
            {
                return BadRequest(new Result()
                {
                    Message = "User not found",
                    Status = false
                });
            }

            var userDto = _mapper.Map<UserDto>(user);

            if (user.Role.ToLower() == "user")
            {
                return Ok(new Result()
                {
                    Data = new { User = userDto },
                    Message = "User Have no Permission",
                    Status = true
                });
            }

            var role = await _roleManager.FindByNameAsync(user.Role);

            var result = await _roleManager.GetClaimsAsync(role);
            var perms = new List<string>();
            if (result != null)
            {
                perms = result.Select(x => x.Value).ToList();
            }

            

            return Ok(new Result()
            {
                Data = new { User = userDto, Permission = perms},
                Status = true
            });
        }


        [HttpPost("Create")]
        public async Task<IActionResult> AddRole(Role role)
        {
            var result = await _unitOfWork.Roles.AddRole(role.Name);
            if (!result.Status) return BadRequest(result);

            var roleDb = await _roleManager.FindByNameAsync(role.Name);

            foreach( var perm in role.Permissions)
            {
                var permName = perm.ToString();
                var roleClaim = new Claim(permName, permName);
                await _roleManager.AddClaimAsync(roleDb, roleClaim);
            }

            return Ok(new Result()
            {
                Status = true,
                Message = "Role Added Successfully"
            });
        }

        [HttpPut("{idd}/Update")]
        public async Task<IActionResult> UpdateRole(Guid idd, Role roleUpdate)
        {
            var role = await _roleManager.FindByIdAsync(idd.ToString());
            var claims = await _roleManager.GetClaimsAsync(role);

            var defaultClaims = Enum.GetNames(typeof(Permission));

            if (role == null)
            {
                return BadRequest(new Result()
                {
                    Message = "Role does not exist",
                    Status = false
                });
            }

            var permClaims = new List<string>();

            foreach(var perm in roleUpdate.Permissions)
            {
                var permName = perm.ToString();
                var roleClaim = new Claim(permName, permName);

                permClaims.Add(permName);

                if(!claims.Where(x=>x.Value == permName).ToList().Any())
                {
                    await _roleManager.AddClaimAsync(role, roleClaim);
                }
            }

            foreach (var claim in claims)
            {
                if (!permClaims.Where(x => x == claim.Value).ToList().Any())
                {
                    await _roleManager.RemoveClaimAsync(role, claim);
                }
            }

            var updateRoleExist = await _roleManager.FindByNameAsync(roleUpdate.Name);
            if (updateRoleExist != null && role.Name != roleUpdate.Name)
            {
                return BadRequest(new Result()
                {
                    Message = "There is already a role with the same name",
                    Status = false
                });
            }

            
            var users = await _context.Users.Where(x => x.Role == role.Name)
                                            .ToListAsync();

            foreach (var user in users)
            {
                user.Role = roleUpdate.Name.ToLower();
            }

            await _roleManager.SetRoleNameAsync(role, roleUpdate.Name);

            await _unitOfWork.SaveAsync();

            return Ok(new Result()
            {
                Message = "Updated Successfully",
                Status = true
            });
        }

        [HttpGet("{idd}")]
        public async Task<IActionResult> GetAllClaimsForRole(Guid idd)
        {
            var role = await _roleManager.FindByIdAsync(idd.ToString());
            if (role == null)
            {
                return BadRequest(new Result()
                { 
                    Message = "Role does not exist",
                    Status = false
                });
            }

            var result = await _roleManager.GetClaimsAsync(role);
            if (result != null)
            {
                var perms = result.Select(x => x.Value).ToList();

                return Ok(new Result()
                {
                    Data = new { Name = role.Name ,Permissions = perms },
                    Status = true
                });
            }

            return BadRequest(new Result()
            {
                Message = "No permssions for this role",
                Status = false
            });
        }

        [HttpGet("All-Permissions")]
        public IActionResult GetAllPerms()
        {
            return Ok(new Result()
            {
                Data = new List<string>() {
                                  "AddDeleteEdit_User",
                                  "AddDeleteEdit_Course",
                                  "AddDeleteEdit_Exp",
                                  "AddDeleteEdit_Role",
                                  "AssignExpToCourse",
                                  "AssignUserToCourse",
                                  "RemoveExpFromCourse",
                                  "RemoveUserFromCourse",
                                  "AssignRoleToUser",
                                  "GetAssignedExp_Student",
                                  "SubmitAssignedExp_Student",
                                  "GetAssignedCourse_Student",
                                  "GetAssignedCourse_Teacher",
                                  "ResetUserPassword"   
                },
                Status = true
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetAllRoles()
        {
            var result = await _unitOfWork.Roles.GetAllUsers();
            return CheckResult(result);
        }

        [HttpPost("{idd}/Assign")]
        public async Task<IActionResult> AddUserToRole(Guid idd, Guid userIdd)
        {
            var user = await _unitOfWork.Users.GetByIdd(userIdd);
            var result = await _unitOfWork.Roles.AddUserToRole(idd, user.Email);

            await _unitOfWork.SaveAsync();
            return CheckResult(result);
        }

        [HttpDelete("{idd}/Remove")]
        public async Task<IActionResult> RemoveRoleFromUser(Guid idd, Guid userIdd)
        {
            var user = await _unitOfWork.Users.GetByIdd(userIdd);
            var result = await _unitOfWork.Roles.RemoveRoleFromUser(idd, user.Email);

            await _unitOfWork.SaveAsync();
            return CheckResult(result);
        }

        [HttpDelete("{idd}/Delete")]
        public async Task<IActionResult> DeleteRole(Guid idd)
        {
            var result = await _unitOfWork.Roles.DeleteRole(idd);
            await _unitOfWork.SaveAsync();
            return CheckResult(result);
        }
    }
}
