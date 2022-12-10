using Aspose.Cells;
using CsvHelper;
using LLS.BLL.IServices;
using LLS.Common.Dto;
using LLS.Common.Dto.Logins;
using LLS.Common.Transfere_Layer_Object;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using static LLS.BLL.Services.UserService;

namespace LLS.API.Controllers
{
    public class UserController : BaseController
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Policy = "AddDeleteEdit_User")]
        [HttpPost("RegisterUsers")]
        public async Task<IActionResult> RegisterUsersAPI(IFormFile usersFile)
        {
            if (usersFile == null || usersFile.Length > 3000000000)
            {
                return BadRequest(new Result()
                {
                    Message = "file error",
                    Status = false
                });
            }

            try
            {
                using var reader = new StreamReader(usersFile.OpenReadStream());
                var ext = System.IO.Path.GetExtension(usersFile.FileName);

                if (ext == ".csv")
                {
                    using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
                    var records = csv.GetRecords<RegisterUser>();

                    var listRes = new List<Result>();

                    foreach (var user in records)
                    {
                        var res = await _userService.RegisterUserWithConfirmToken(user, "student");
                        listRes.Add(res);
                    }

                    return Ok(listRes);
                }

                //if(ext == ".xlsx")
                //{
                //    var workbook = new Workbook(usersFile.OpenReadStream());

                //    using var csv = new CsvReader(workbook.SaveToStream(), CultureInfo.InvariantCulture);
                //    var records = csv.GetRecords<RegisterUser>();

                //    var listRes = new List<Result>();

                //    foreach (var user in records)
                //    {
                //        var res = await _userService.RegisterUserWithConfirmToken(user);
                //        listRes.Add(res);
                //    }

                //    return Ok(listRes);
                //}

                return BadRequest(new Result()
                {
                    Status = false,
                    Message = "Invalid file exctention"
                });
            }
            catch(Exception ex)
            {
                return BadRequest(new Result()
                {
                    Status = false,
                    Message = ex.Message.ToString()
                });
            }
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Policy = "AddDeleteEdit_User")]
        [HttpPost("RegisterUser")]
        public async Task<IActionResult> RegisterUserAPI(UpdateUser registerUser)
        {
            var reg = new RegisterUser()
            {
                Email = registerUser.email,
                FirstName = registerUser.firstName,
                LastName = registerUser.lastName,
                City = registerUser.City,
                Country = registerUser.country,
                AcademicYear = registerUser.academicYear,
                Gender = registerUser.Gender,
                PhoneNumber = registerUser.Gender
            };
            var res = await _userService.RegisterUserWithConfirmToken(reg, registerUser.Role);
            
            return CheckResult(res);
        }

        [HttpPost("SignUp")]
        public async Task<IActionResult> SingUp(SignUp signUp)
        {
            if(ModelState.IsValid)
            {
                var res = await _userService.AddUser(signUp);
                return CheckResult(res);
            }

            return BadRequest(new Result
            {
                Status = false,
                Message = "Invalid Payload"
            });
        }

        //To Do remove email and make it with JWT only
        [HttpGet("email")]
        public async Task<IActionResult> GetUserByEmail(string email)
        {
            var res = await _userService.GetUserByEmail(email);

            return CheckResult(res);
        }

        [HttpGet("{userIdd}")]
        public async Task<IActionResult> GetUserByIdd(Guid userIdd)
        {
            var res = await _userService.GetUserByIdd(userIdd);

            return CheckResult(res);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUser()
        {
            var res = await _userService.GetAllUsers();

            return CheckResult(res);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Policy = "AddDeleteEdit_User")]
        [HttpDelete("Delete")]
        public async Task<IActionResult> DeleteUser(Guid userIdd)
        {
            var res = await _userService.DeleteUser(userIdd);

            return CheckResult(res);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Policy = "AddDeleteEdit_User")]
        [HttpPut("{idd}/Update")]
        public async Task<IActionResult> UpdateUser(Guid idd,UpdateUser updateUser)
        {
            var userDto = new UserDto()
            {
                Idd = idd,
                FirstName = updateUser.firstName,
                Lastname = updateUser.lastName,
                AcademicYear = updateUser.academicYear,
                PhoneNumber = updateUser.phoneNumber,
                Country = updateUser.country,
                City = updateUser.City,
                Gender = updateUser.Gender,
                Role = updateUser.Role
            };

            var res = await _userService.UpdateUser(userDto);

            return CheckResult(res);
        }

        [HttpGet("Get-All-Users-With-Role")]
        public async Task<IActionResult> GetAllUsersWithRole(string role)
        {
            var res = await _userService.GetAllUserWithRole(role);

            return CheckResult(res);
        }
    }

    public class UpdateUser
    {
        public string email { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string phoneNumber { get; set; }
        public string country { get; set; }
        public string academicYear { get; set; }
        public string City { get; set; }
        public string Gender { get; set; }
        public string Role { get; set; }
    }

}
