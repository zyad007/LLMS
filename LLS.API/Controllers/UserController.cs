using Aspose.Cells;
using CsvHelper;
using LLS.BLL.IServices;
using LLS.Common.Dto;
using LLS.Common.Dto.Logins;
using LLS.Common.Transfere_Layer_Object;
using LLS.DAL.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using static LLS.BLL.Services.UserService;

namespace LLS.API.Controllers
{
    public class UserController : BaseController
    {
        private readonly IUserService _userService;
        private static IWebHostEnvironment _enviroment;
        private readonly AppDbContext _context;
        public UserController(IUserService userService, IWebHostEnvironment enviroment,AppDbContext context)
        {
            _userService = userService;
            _enviroment = enviroment;
            _context = context;
        }

        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
        //    Policy = "AddDeleteEdit_User")]
        //[HttpPost("RegisterUsers")]
        //public async Task<IActionResult> RegisterUsersAPI(IFormFile usersFile)
        //{
        //    if (usersFile == null || usersFile.Length > 3000000000)
        //    {
        //        return BadRequest(new Result()
        //        {
        //            Message = "file error",
        //            Status = false
        //        });
        //    }

        //    try
        //    {
        //        using var reader = new StreamReader(usersFile.OpenReadStream());
        //        var ext = System.IO.Path.GetExtension(usersFile.FileName);

        //        if (ext == ".csv")
        //        {
        //            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        //            var records = csv.GetRecords<RegisterUser>();

        //            var listRes = new List<Result>();

        //            foreach (var user in records)
        //            {
        //                var res = await _userService.RegisterUserWithConfirmToken(user, "student");
        //                listRes.Add(res);
        //            }

        //            return Ok(listRes);
        //        }

        //        //if(ext == ".xlsx")
        //        //{
        //        //    var workbook = new Workbook(usersFile.OpenReadStream());

        //        //    using var csv = new CsvReader(workbook.SaveToStream(), CultureInfo.InvariantCulture);
        //        //    var records = csv.GetRecords<RegisterUser>();

        //        //    var listRes = new List<Result>();

        //        //    foreach (var user in records)
        //        //    {
        //        //        var res = await _userService.RegisterUserWithConfirmToken(user);
        //        //        listRes.Add(res);
        //        //    }

        //        //    return Ok(listRes);
        //        //}

        //        return BadRequest(new Result()
        //        {
        //            Status = false,
        //            Message = "Invalid file exctention"
        //        });
        //    }
        //    catch(Exception ex)
        //    {
        //        return BadRequest(new Result()
        //        {
        //            Status = false,
        //            Message = ex.Message.ToString()
        //        });
        //    }
        //}

        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
        //    Policy = "AddDeleteEdit_User")]
        //[HttpPost("RegisterUser")]
        //public async Task<IActionResult> RegisterUserAPI(UpdateUser registerUser)
        //{
        //    var reg = new RegisterUser()
        //    {
        //        Email = registerUser.email,
        //        FirstName = registerUser.firstName,
        //        LastName = registerUser.lastName,
        //        City = registerUser.City,
        //        Country = registerUser.country,
        //        AcademicYear = registerUser.academicYear,
        //        Gender = registerUser.Gender,
        //        PhoneNumber = registerUser.Gender
        //    };
        //    var res = await _userService.RegisterUserWithConfirmToken(reg, registerUser.Role);

        //    return CheckResult(res);
        //}

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Policy = "AddDeleteEdit_User")]
        [HttpPost]
        public async Task<IActionResult> SingUp(RegisterUser signUp)
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
        //[HttpGet("email")]
        //public async Task<IActionResult> GetUserByEmail(string email)
        //{
        //    var res = await _userService.GetUserByEmail(email);

        //    return CheckResult(res);
        //}

        [HttpGet("{idd}")]
        public async Task<IActionResult> GetUserByIdd(Guid idd)
        {
            var res = await _userService.GetUserByIdd(idd);

            return CheckResult(res);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUser(Guid courseIdd,[FromQuery] int page, [FromQuery] string role,string searchByEmail, string searchByFirstName, string searchByLastName)
        {
            var res = await _userService.GetAllUsers(courseIdd, page-1, role,searchByEmail, searchByFirstName, searchByLastName);

            return CheckResult(res);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Policy = "AddDeleteEdit_User")]
        [HttpDelete("{idd}")]
        public async Task<IActionResult> DeleteUser(Guid idd)
        {
            var res = await _userService.DeleteUser(idd);

            return CheckResult(res);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Policy = "AddDeleteEdit_User")]
        [HttpPut("{idd}")]
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
                Role = updateUser.Role,
                imgURL = updateUser.ImgURL
            };

            var res = await _userService.UpdateUser(userDto);

            return CheckResult(res);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Policy = "AddDeleteEdit_User")]
        [HttpPost("Profile")]
        public IActionResult UploadImage(IFormFile imageUpload)
        {
            var objFile = imageUpload;

            string host = HttpContext.Request.Host.Value;

            if (objFile == null || objFile.Length > 3000000000)
            {
                return BadRequest(new
                {
                    uploaded = false,
                    url = ""
                });
            }

            if (!Directory.Exists(_enviroment.WebRootPath + "/Images/"))
            {
                Directory.CreateDirectory(_enviroment.WebRootPath + "/Images/");
            }

            string baseUrl = _enviroment.WebRootPath + "/Images/";

            string fileName = objFile.FileName;

            string newFileName = Guid.NewGuid().ToString() + Path.GetExtension(fileName);
            string newFilePath = baseUrl + newFileName;

            using (var fileStream = new FileStream(newFilePath, FileMode.Create))
            {
                objFile.CopyTo(fileStream);
            }

            var imageUrl = "https://" + host + "/Images/" + newFileName;

            return Ok(new
            {
                uploaded = true,
                url = imageUrl
            });
        }

        //[HttpGet("Get-All-Users-With-Role")]
        //public async Task<IActionResult> GetAllUsersWithRole(string role)
        //{
        //    var res = await _userService.GetAllUserWithRole(role);

        //    return CheckResult(res);
        //}
    }

    

}
