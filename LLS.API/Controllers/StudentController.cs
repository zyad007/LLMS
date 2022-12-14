using LLS.BLL.IServices;
using LLS.Common.Dto;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace LLS.API.Controllers
{
    public class StudentController : BaseController
    {
        private readonly IStudentService _sudentService;
        public StudentController(IStudentService sudentService)
        {
            _sudentService = sudentService;
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Policy = "GetAssignedExp_Student")]
        [HttpGet("Assigned-Experiments")]
        public async Task<IActionResult> GetAllExpAssignedToStudent()
        {
            var userEmail = HttpContext.User.FindFirst(ClaimTypes.Email).Value;

            var res = await _sudentService.GetAssignedExpForStudent(userEmail);

            return CheckResult(res);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Policy = "GetAssignedCourse_Student")]
        [HttpGet("Completed-Experiments")]
        public async Task<IActionResult> GetAllCompletedExp()
        {
            var userEmail = HttpContext.User.FindFirst(ClaimTypes.Email).Value;

            var res = await _sudentService.GetCompletedExp(userEmail);

            return CheckResult(res);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Policy = "GetAssignedCourse_Student")]
        [HttpGet("Assigned-Courses")]
        public async Task<IActionResult> GetStudentCourses()
        {
            var userEmail = HttpContext.User.FindFirst(ClaimTypes.Email).Value;

            var res = await _sudentService.GetStudentCourses(userEmail);

            return CheckResult(res);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Policy = "GetAssignedCourse_Student")]
        [HttpPost("Submit")]
        public async Task<IActionResult> SubmintExp(StudentSubmit studentSubmit)
        {
            var userEmail = HttpContext.User.FindFirst(ClaimTypes.Email).Value;

            var res = await _sudentService.SubmitExp(studentSubmit, userEmail);

            return CheckResult(res);
        }

        //[HttpGet("Result")]
        //public async Task<IActionResult> GetStudentResults(string email, string courseIdd, Guid expIdd)
        //{
        //    var res = await _sudentService.GetStudentResult(email, courseIdd, expIdd);

        //    return CheckResult(res);
        //}

        //[HttpPost("Resrve-TimeSlot")]
        //public async Task<IActionResult> ReserveTimeSlotAPI(string email, Guid expIdd, string courseIdd, int timeSlot)
        //{
        //    var res = await _sudentService.ReserveTimeSlot(email, expIdd, courseIdd, timeSlot);

        //    return CheckResult(res);
        //}



    }
}
