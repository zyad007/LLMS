using AutoMapper;
using LLS.BLL.IServices;
using LLS.Common.Dto;
using LLS.Common.Dto.Logins;
using LLS.Common.Models;
using LLS.Common.Transfere_Layer_Object;
using LLS.DAL.Data;
using LLS.DAL.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
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
        private readonly IAccountService _accountService;
        private readonly AppDbContext _context;
        public UserService(IUnitOfWork unitOfWork,
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IMapper iMapper,
            IAccountService accountService,
            AppDbContext context)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _roleManager = roleManager;
            _iMapper = iMapper;
            _accountService = accountService;
            _context = context;
        }



        public async Task<Result> AddUser(RegisterUser user)
        {
            var passwordValidator = new PasswordValidator<IdentityUser>();
            var passwRes = await passwordValidator.ValidateAsync(_userManager,null,user.Password);
            if(!passwRes.Succeeded)
            {
                return new Result()
                {
                    Message = "Invalid Password",
                    Status = false
                };
            }


            //Check if Email already Exisit
            var userExist = await _unitOfWork.Users.GetByEmail(user.Email);

            if (userExist != null) //Email exists
            {
                return new Result()
                {
                    Message = $"[{user.Email}]: Email already in use",
                    Status = false
                };
            }

            //Add the user
            var newUser = new IdentityUser()
            {
                Email = user.Email,
                UserName = user.Email,
                EmailConfirmed = false // To be changed to confirmed by email
            };

            var isCreated = await _userManager.CreateAsync(newUser);
            if (!isCreated.Succeeded)
            {
                return new Result()
                {
                    Message = $"[{user.Email}]: {isCreated.Errors.FirstOrDefault().Description}",
                    Status = false
                };
            }

            //Add user to Db
            var userDb = new User();
            userDb.IdentityId = new Guid(newUser.Id);
            userDb.FirstName = user.FirstName;
            userDb.Lastname = user.LastName;
            userDb.Country = user.Country;
            userDb.PhoneNumber = user.PhoneNumber;
            userDb.AcademicYear = user.AcademicYear;
            userDb.Gender = user.Gender;
            userDb.City = user.City;
            userDb.Email = user.Email;
            userDb.imgURL = user.ImgURL;

            if(String.IsNullOrEmpty(user.Role)) 
            {
                user.Role = "none";
            }

            var exist = await _roleManager.FindByNameAsync(user.Role);
            if (exist != null) userDb.Role = user.Role.ToUpper();

            var result = await _unitOfWork.Users.Create(userDb);

            var idUser = await _userManager.FindByEmailAsync(userDb.Email);

            await _userManager.AddPasswordAsync(idUser ,user.Password);
            //var token = await _userManager.GenerateEmailConfirmationTokenAsync(idUser);

            var exists = await _roleManager.FindByNameAsync(user.Role);
            if (exists != null) await _userManager.AddToRoleAsync(idUser, user.Role);

            await _unitOfWork.SaveAsync();

            var userDto = _iMapper.Map<UserDto>(userDb);


            return new Result()
            {
                Status = true,
                Message = "User has been created Successfully",
                Data = userDto
            };
        }
        
        public async Task<Result> RegisterUserWithConfirmToken(RegisterUser user, string role)
        {
            //Check if Email already Exisit
            var userExist = await _unitOfWork.Users.GetByEmail(user.Email);

            if (userExist != null) //Email exists
            {
                return new Result()
                {
                    Data = new {email = user.Email},
                    Message = $"[{user.Email}]: Email already in use",
                    Status = false
                };
            }

            //Add the user
            var newUser = new IdentityUser()
            {
                Email = user.Email,
                UserName = user.Email,
                EmailConfirmed = false 
            };

            var isCreated = await _userManager.CreateAsync(newUser);
            if (!isCreated.Succeeded)
            {
                return new Result()
                {
                    Data = new { email = user.Email },
                    Message = $"[{user.Email}]: {isCreated.Errors.FirstOrDefault().Description}",
                    Status = false
                };
            }

            //Add user to Db
            var userDb = new User();
            userDb.IdentityId = new Guid(newUser.Id);
            userDb.FirstName = user.FirstName;
            userDb.Lastname = user.LastName;
            userDb.Country = user.Country;
            userDb.PhoneNumber = user.PhoneNumber;
            userDb.AcademicYear = user.AcademicYear;
            userDb.Gender = user.Gender;
            userDb.City = user.City;
            userDb.Email = user.Email;
            var exist = await _roleManager.FindByNameAsync(role);
            if (exist != null) userDb.Role = role.ToUpper();

            var result = await _unitOfWork.Users.Create(userDb);

            //Add Default Role to User
            //if (await _roleManager.RoleExistsAsync("user") == true)
            //{
            //    await _unitOfWork.Roles.AddRole("user");
            //}
            //await _userManager.AddToRoleAsync(newUser, "user");

            //Add Role to User
            //var roleResult = await _unitOfWork.Roles.AddUserToRole("user", userDb.Email);

            var idUser = await _userManager.FindByEmailAsync(userDb.Email);

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(idUser);

            var exists = await _roleManager.FindByNameAsync(role);
            if(exists != null) await _userManager.AddToRoleAsync(idUser, role);


            await _unitOfWork.SaveAsync();

            //Send Confimation Email to set password


            return new Result { 
                Message = $"[{user.Email}]: Added successfully" ,
                Data = new {email = user.Email, token = token},
                Status = true
            };
        }

        public async Task<AuthResult> ConfirmAccount(string email, string password, string token)
        {
            var idUser = await _userManager.FindByEmailAsync(email);
            if (idUser == null)
            {
                return new AuthResult()
                {
                    Status = false,
                    Error = "User doesn't exist"
                };
            }

            var isConfirmed = await _userManager.ConfirmEmailAsync(idUser,token);
            if(!isConfirmed.Succeeded)
            {
                return new AuthResult()
                {
                    Status = false,
                    Error = isConfirmed.Errors.Select(x => x.Description).ToList()
                };
            }
            
            var isSetPassword = await _userManager.AddPasswordAsync(idUser, password);
            if (!isSetPassword.Succeeded)
            {
                return new AuthResult()
                {
                    Status = false,
                    Error = isSetPassword.Errors.Select(x => x.Description).ToList()
                };
            }

            await _unitOfWork.SaveAsync();

            return await _accountService.Login(new Login { Email = email, Password = password });
        }

        public async Task<Result> GetAllUsers(Guid courseIdd, int page, string role, string searchByEmail, string searchByFirstName, string searchByLastName)
        {
            searchByEmail += "";
            searchByFirstName += "";
            searchByLastName += "";

            if (page == -1)
            {
                page = 0;
            }

            var users = await _unitOfWork.Users.GetAll();

            var filterdUser = new List<User>();

            if(courseIdd != Guid.Empty)
            {
                var course = await _context.Courses.FirstOrDefaultAsync(x => x.Idd == courseIdd);

                if(course == null)
                {
                    return new Result()
                    {
                        Message = "No course with tihs IDD",
                        Status = false
                    };
                }

                foreach (var user in users)
                {
                    var isAssigned = await _context.User_Courses.FirstOrDefaultAsync(x => x.CourseId == course.Id && x.UserId == user.Id);
                    
                    if(isAssigned == null)  filterdUser.Add(user);    
                }
            }else
            {
                filterdUser = users;
            }

            var usersDto = new List<UserDto>();

            foreach(var user in filterdUser)
            {
                var userDto = _iMapper.Map<UserDto>(user);
                usersDto.Add(userDto);
            }
            
            if(!String.IsNullOrEmpty(role)) usersDto = usersDto.Where(x => x.Role.ToLower() == role.ToLower()).ToList();

            var searchedUsers = usersDto.Where(x => x.Email.ToLower().Contains(""+searchByEmail.ToLower()) &&
            x.FirstName.ToLower().Contains("" + searchByFirstName.ToLower()) &&
            x.Lastname.ToLower().Contains("" + searchByLastName.ToLower())).ToList();
            var paginUsers = searchedUsers.Skip(page * 10).Take(10).ToList();
            int count = searchedUsers.Count;


            return new Result()
            {
                Data = new
                {
                    result = paginUsers,
                    count,
                    next = (page * 10) + 10 >= count ? null : $"https://optimistic-burnell.74-50-88-98.plesk.page/api/User?courseIdd={courseIdd}&page={page + 1+1}",
                    previous = page == 0 ? null : $"https://optimistic-burnell.74-50-88-98.plesk.page/api/User?courseIdd={courseIdd}&page={page - 1+1}"
                },
                Status = true
            };
        }

        public async Task<Result> GetUserByEmail(string idd)
        {
            var user = await _unitOfWork.Users.GetByEmail(idd);
            if(user == null)
            {
                return new Result()
                {
                    Status = false,
                    Message = "User doesn't exist"
                };
            }

            var userDto = _iMapper.Map<UserDto>(user);

            return new Result()
            {
                Status = true,
                Data = userDto
            };
        }

        public async Task<Result> GetUserByIdd(Guid userIdd)
        {
            var user = await _unitOfWork.Users.GetByIdd(userIdd);
            if (user == null)
            {
                return new Result()
                {
                    Status = false,
                    Message = "User doesn't exist"
                };
            }

            var userDto = _iMapper.Map<UserDto>(user);

            return new Result()
            {
                Status = true,
                Data = userDto
            };
        }

        public async Task<Result> DeleteUser(Guid userIdd)
        {
            var user = await _unitOfWork.Users.GetByIdd(userIdd);
            if (user == null)
            {
                return new Result()
                {
                    Status = false,
                    Message = "User doesn't exist"
                };
            }

            var email = user.Email;

            _unitOfWork.Users.Delete(user);

            var userI = await _userManager.FindByEmailAsync(email);

            await _userManager.DeleteAsync(userI);

            await _unitOfWork.SaveAsync();

            return new Result()
            {
                Status = true,
                Message = "User Deleted successfully"
            };
        }

        public async Task<Result> UpdateUser(UserDto userDto)
        {
            var user = await _unitOfWork.Users.GetByIdd(userDto.Idd);
            if (user == null)
            {
                return new Result()
                {
                    Status = false,
                    Message = "User doesn't exist"
                };
            }

            if(user.Role != userDto.Role)
            {
                if(userDto.Role != "none")
                {
                    var role = await _roleManager.FindByNameAsync(userDto.Role);
                    if (role == null)
                    {
                        return new Result()
                        {
                            Status = false,
                            Message = "no role with this name"
                        };
                    }

                    var idUser = await _userManager.FindByEmailAsync(user.Email);
                    await _userManager.RemoveFromRoleAsync(idUser, user.Role);
                    await _userManager.AddToRoleAsync(idUser, userDto.Role);
                }else
                {
                    var idUser = await _userManager.FindByEmailAsync(user.Email);
                    await _userManager.RemoveFromRoleAsync(idUser, user.Role);
                }
            }

            var newUser = _iMapper.Map<User>(userDto);

            await _unitOfWork.Users.Update(newUser);

            await _unitOfWork.SaveAsync();

            var userRes = await _unitOfWork.Users.GetByIdd(userDto.Idd);

            return new Result()
            {
                Status = true,
                Message = "User Updated Seuccessfully",
                Data = _iMapper.Map<UserDto>(userRes)
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

        public class RegisterUser
        {
            public string Email { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Country { get; set; }
            public string PhoneNumber { get; set; }
            public string AcademicYear { get; set; }
            public string City { get; set; }
            public string Gender { get; set; }
            public string Role { get; set; }
            public string Password { get; set; }
            public string ImgURL { get; set; }
        }
    }
}
