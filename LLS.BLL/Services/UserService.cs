using AutoMapper;
using LLS.BLL.IServices;
using LLS.Common.Dto;
using LLS.Common.Dto.Logins;
using LLS.Common.Models;
using LLS.Common.Transfere_Layer_Object;
using LLS.DAL.Interfaces;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLS.BLL.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMapper _iMapper;
        public UserService(IUnitOfWork unitOfWork,
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IMapper iMapper)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _roleManager = roleManager;
            _iMapper = iMapper;
        }

        public async Task<Result> AddUser(SignUp signUp)
        {
            //Check if Email already Exisit
            var userExist = await _unitOfWork.Users.GetByEmail(signUp.Email);

            if (userExist != null) //Email exists
            {
                return new Result()
                {
                    Status = false,
                    Message = "Email already in use"
                };
            }

            //Check if Role Exist
            var roleExist = await _roleManager.RoleExistsAsync(signUp.Role);
            if (!roleExist)
            {
                return new Result()
                {
                    Message = "Role does not exist",
                    Status = false
                };
            }

            //Add the user
            var newUser = new IdentityUser()
            {
                Email = signUp.Email,
                UserName = signUp.Email,
                EmailConfirmed = true //To Update to confirm email

            };

            var isCreated = await _userManager.CreateAsync(newUser, signUp.Password);
            if (!isCreated.Succeeded)
            {
                return new Result()
                {
                    Status = false,
                    Message = isCreated.Errors.Select(x => x.Description).ToList()
                };
            }

            //Add user to Db
            var user = new User();
            user.IdentityId = new Guid(newUser.Id);
            user.FirstName = signUp.FirstName;
            user.Lastname = signUp.LastName;
            user.Email = signUp.Email;

            var result = await _unitOfWork.Users.Create(user);

            //Add Default Role to User
            if (await _roleManager.RoleExistsAsync("User") == true)
            {
                await _unitOfWork.Roles.AddRole("User");
            }
            await _userManager.AddToRoleAsync(newUser, "User");

            //Add Role to User
            var roleResult = await _unitOfWork.Roles.AddUserToRole(signUp.Role.ToLower(), signUp.Email);
            if (roleResult.Status == false)
            {
                return new Result()
                {
                    Status = false,
                    Message = roleResult.Message
                };
            }

            await _unitOfWork.SaveAsync();

            return new Result()
            {
                Status = true,
                Message = "User has been created Successfully",
                Data = _iMapper.Map<UserDto>(user)
            };
        }

        public async Task<Result> GetAllUsers()
        {
            var users = await _unitOfWork.Users.GetAll();

            var usersDto = new List<UserDto>();

            foreach(var user in users)
            {
                var userDto = _iMapper.Map<UserDto>(user);
                usersDto.Add(userDto);
            }

            return new Result()
            {
                Data = usersDto,
                Status = true
            };
        }

        public async Task<Result> GetUserByEmail(string email)
        {
            var user = await _unitOfWork.Users.GetByEmail(email);
            if(user == null)
            {
                return new Result()
                {
                    Status = false,
                    Message = "No User doesn't exist"
                };
            }

            var userDto = _iMapper.Map<UserDto>(user);

            return new Result()
            {
                Status = true,
                Data = userDto
            };
        }

        public async Task<Result> DeleteUser(string email)
        {
            var user = await _unitOfWork.Users.GetByEmail(email);
            if (user == null)
            {
                return new Result()
                {
                    Status = false,
                    Message = "User doesn't exist"
                };
            }

            _unitOfWork.Users.Delete(user);
            await _unitOfWork.SaveAsync();

            return new Result()
            {
                Status = true,
                Message = "User Deleted successfully"
            };
        }

        public async Task<Result> UpdateUser(UserDto userDto)
        {
            var user = await _unitOfWork.Users.GetByEmail(userDto.Email);
            if (user == null)
            {
                return new Result()
                {
                    Status = false,
                    Message = "User doesn't exist"
                };
            }

            var newUser = _iMapper.Map<User>(userDto);

            await _unitOfWork.Users.Update(newUser);
            await _unitOfWork.SaveAsync();

            return new Result()
            {
                Status = true,
                Message = "User Updated Seuccessfully",
                Data = userDto
            };
        }

        public async Task<Result> GetAllUserWithRole(string role)
        {
            var roleExist = await _roleManager.RoleExistsAsync(role);
            if (!roleExist)
            {
                return new Result()
                {
                    Message = "Role does not exist",
                    Status = false
                };
            }

            var users = await _unitOfWork.Users.GetAll();

            var wanted = users.Where(x => x.Role.ToLower() == role.ToLower());
            var userDtos = new List<UserDto>();
            foreach(var user in wanted)
            {
                var userDto = _iMapper.Map<UserDto>(user);
                userDtos.Add(userDto);
            }

            return new Result()
            {
                Status = true,
                Data = userDtos
            };

        }
    }
}
