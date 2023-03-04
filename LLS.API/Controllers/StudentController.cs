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
using static LLS.BLL.Services.StudentService;

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
        public async Task<IActionResult> GetAllExpAssignedToStudent([FromQuery] int page)
        {
            var userEmail = HttpContext.User.FindFirst(ClaimTypes.Email).Value;

            var res = await _sudentService.GetAssignedExpForStudent(userEmail, page - 1);

            return CheckResult(res);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Policy = "GetAssignedCourse_Student")]
        [HttpGet("Completed-Experiments")]
        public async Task<IActionResult> GetAllCompletedExp([FromQuery] int page)
        {
            var userEmail = HttpContext.User.FindFirst(ClaimTypes.Email).Value;

            var res = await _sudentService.GetCompletedExp(userEmail, page - 1);

            return CheckResult(res);
        }

        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
        //    Policy = "GetAssignedCourse_Student")]
        //[HttpGet("Assigned-Courses")]
        //public async Task<IActionResult> GetStudentCourses()
        //{
        //    var userEmail = HttpContext.User.FindFirst(ClaimTypes.Email).Value;

        //    var res = await _sudentService.GetStudentCourses(userEmail);

        //    return CheckResult(res);
        //}

        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
        //    Policy = "GetAssignedCourse_Student")]
        //[HttpPost("Submit")]
        //public async Task<IActionResult> SubmintExp(StudentSubmit studentSubmit)
        //{
        //    var userEmail = HttpContext.User.FindFirst(ClaimTypes.Email).Value;

        //    var res = await _sudentService.SubmitExp(studentSubmit, userEmail);

        //    return CheckResult(res);
        //}

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Policy = "GetAssignedCourse_Student")]
        [HttpPost("Start-Trial")]
        public async Task<IActionResult> StartTrial(Guid courseIdd, Guid expIdd)
        {
            var studentIdId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var res = await _sudentService.startTrial(courseIdd, expIdd, studentIdId);

            return CheckResult(res);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Policy = "GetAssignedCourse_Student")]
        [HttpPost("Submit-Trial")]
        public async Task<IActionResult> SubmitTrial(TrialSubmit studentSubmit)
        {
            var studentIdId = HttpContext.User.FindFirst(ClaimTypes.Email).Value;

            var res = await _sudentService.SubmitTrial(studentSubmit, studentIdId);

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
