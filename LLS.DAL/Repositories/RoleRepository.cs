using LLS.Common.Models;
using LLS.Common.Transfere_Layer_Object;
using LLS.DAL.Data;
using LLS.DAL.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLS.DAL.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly AppDbContext _context;

        public RoleRepository(UserManager<IdentityUser> userManager,
                                RoleManager<IdentityRole> roleManager,
                                AppDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        public async Task<Result> AddRole(string roleName)
        {
            var roleExist = await _roleManager.FindByNameAsync(roleName.ToLower());

            if (roleExist == null)
            {
                var result = await _roleManager.CreateAsync(new IdentityRole(roleName));

                if (result.Succeeded)
                {
                    return new Result()
                    {
                        Data = $"The Role {roleName} has been added successfully",
                        Status = true
                    };
                }
                return new Result()
                {
                    Message = $"The Role {roleName} has not been added",
                    Status = false
                };
            }

            return new Result()
            {
                Message = "Role already exists",
                Status = false
            };
        }

        public async Task<Result> GetAllUsers()
        {
            var result = await _roleManager.Roles.ToListAsync();
            if (result.Any())
            {
                return new Result()
                {
                    Data = result,
                    Status = true
                };
            }
            return new Result()
            {
                Message = "There is no Roles",
                Status = false
            };
        }

        public async Task<Result> AddUserToRole(Guid idd, string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return new Result()
                {
                    Message = "User does not exist",
                    Status = false
                };
            }

            var roleExist = await _roleManager.FindByIdAsync(idd.ToString());
            if (roleExist == null)
            {
                return new Result()
                {
                    Message = "Role does not exist",
                    Status = false
                };
            }

            var roleName = roleExist.Name;
            var userRoles = await _userManager.GetRolesAsync(user);

            if (userRoles.Count == 1)
            {
                if (userRoles[0].ToLower() == roleName.ToLower())
                {
                    return new Result()
                    {
                        Message = "User already got this role",
                        Status = false
                    };
                } 
                else
                {
                    await _userManager.RemoveFromRoleAsync(user, userRoles[0].ToLower());
                }
            }

            var result = await _userManager.AddToRoleAsync(user, roleName);
            if (!result.Succeeded)
            {
                return new Result()
                {
                    Message = "Couldnt Assign this user to this role",
                    Status = false
                };
            }

            //Add Role in User model Db
            var id = new Guid(user.Id);
            var userModel = await GetUserByIdentityId(id);
            userModel.Role = roleName.ToUpper();
            var updateUserRole = await UpdateIdentityId(id, userModel);
            if (!updateUserRole)
            {
                return new Result()
                {
                    Message = "Couldnt Assign this user to this role",
                    Status = false
                };
            }

            return new Result()
            {
                Data = $"The role {roleName} added to the user successfully",
                Status = true
            };
        }
        public async Task<Result> GetUserRoles(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return new Result()
                {
                    Message = "User does not exist",
                    Status = false
                };
            }

            var roles = await _userManager.GetRolesAsync(user);

            return new Result()
            {
                Data = roles,
                Status = true
            };
        }
        public async Task<Result> RemoveRoleFromUser(Guid idd, string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return new Result()
                {
                    Message = "User Doesn't Exist",
                    Status = false
                };
            }

            var roleExist = await _roleManager.FindByIdAsync(idd.ToString());
            if (roleExist == null)
            {
                return new Result()
                {
                    Message = "Role Doesn't Exist",
                    Status = false
                };
            }

            var roleName = roleExist.Name;
            var userRoles = await _userManager.GetRolesAsync(user);

            if (userRoles.Count() == 1 && roleName.ToLower() != userRoles[0].ToLower())
            {
                return new Result()
                {
                    Message = "User Doesn't have this role",
                    Status = false
                };
            }

            var result = await _userManager.RemoveFromRoleAsync(user, roleName);
            if (!result.Succeeded)
            {
                return new Result()
                {
                    Message = $"Unable to remove User form role {roleName}",
                    Status = false
                };
            }

            //Add Role in User model Db
            var id = new Guid(user.Id);
            var userModel = await GetUserByIdentityId(id);
            userModel.Role = "USER";
            var updateUserRole = await UpdateIdentityId(id, userModel);
            if (!updateUserRole)
            {
                return new Result()
                {
                    Message = $"Unable to remove User form role {roleName}",
                    Status = false
                };
            }

            return new Result()
            {
                Data = $"The role {roleName} removed from the user successfully",
                Status = true
            };
        }
        public async Task<Result> DeleteRole(Guid idd)
        {
            var role = await _roleManager.FindByIdAsync(idd.ToString());
            if (role == null)
            {
                return new Result()
                {
                    Message = "Role Doesn't Exist",
                    Status = false
                };
            }

            var roleName = role.Name;

            var result = await _roleManager.DeleteAsync(role);
            if (!result.Succeeded)
            {
                return new Result()
                {
                    Message = "Unable to Delete Role",
                    Status = false
                };
            }

            var resultDb = await UserDeleteRole(roleName);
            if (!resultDb)
            {
                return new Result()
                {
                    Message = "Unable to Delete Role",
                    Status = false
                };
            }

            return new Result()
            {
                Data = $"The role {roleName} have been deleted successfully",
                Status = true
            };

        }
        private async Task<User> GetUserByIdentityId(Guid identityId)
        {
            var user = await _context.Users.Where(x => x.IdentityId == identityId)
                                            .FirstOrDefaultAsync();
            return user;
        }

        private async Task<bool> UpdateIdentityId(Guid id, User newUser)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.IdentityId == id);
            if (user != null)
            {
                user.FirstName = newUser.FirstName;
                user.Lastname = newUser.Lastname;
                user.Email = newUser.Email;
                user.Country = newUser.Country;
                user.PhoneNumber = newUser.PhoneNumber;
                user.Gender = newUser.Gender;
                user.AcademicYear = newUser.AcademicYear;
                user.City = newUser.City;

                user.Role = newUser.Role;

                user.UpdateDate = DateTime.UtcNow;

                return true;
            }
            return false;
        }

        private async Task<bool> UserDeleteRole(string roleName)
        {
            var users = await _context.Users.Where(x => x.Role.ToLower() == roleName.ToLower())
                                            .ToListAsync();
            if (users == null)
            {
                return false;
            }

            foreach (var user in users)
            {
                user.Role = "USER";
            }

            return true;
        }
    }
    
}
