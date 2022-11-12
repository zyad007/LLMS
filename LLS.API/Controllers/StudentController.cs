using LLS.BLL.IServices;
using LLS.Common.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
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

        [HttpGet("Experiments")]
        public async Task<IActionResult> GetAllExpAssignedToStudent(string email)
        {
            var res = await _sudentService.GetAssignedExpForStudent(email);

            return CheckResult(res);
        }

        [HttpGet("Courses")]
        public async Task<IActionResult> GetStudentCourses(string email)
        {
            var res = await _sudentService.GetStudentCourses(email);

            return CheckResult(res);
        }

        [HttpPost("Submit")]
        public async Task<IActionResult> SubmintExp(StudentSubmit studentSubmit)
        {
            var res = await _sudentService.SubmitExp(studentSubmit);

            return CheckResult(res);
        }

        [HttpGet("Result")]
        public async Task<IActionResult> GetStudentResults(string email, string courseIdd, Guid expIdd)
        {
            var res = await _sudentService.GetStudentResult(email, courseIdd, expIdd);

            return CheckResult(res);
        }

        [HttpPost("Resrve-TimeSlot")]
        public async Task<IActionResult> ReserveTimeSlotAPI(string email, Guid expIdd, string courseIdd, int timeSlot)
        {
            var res = await _sudentService.ReserveTimeSlot(email, expIdd, courseIdd, timeSlot);

            return CheckResult(res);
        }



    }
}
