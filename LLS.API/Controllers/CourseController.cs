using LLS.BLL.IServices;
using LLS.Common.Dto;
using LLS.Common.Transfere_Layer_Object;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.HttpSys;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LLS.API.Controllers
{
    
    public class CourseController : BaseController
    {
        private readonly ICourseService _courseService;
        public CourseController(ICourseService courseService)
        {
            _courseService = courseService;
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Policy = "AddDeleteEdit_Course")]
        [HttpPost("Create")]
        public async Task<IActionResult> CreateCourse(CourseDto courseDto)
        {
            courseDto.Idd = Guid.NewGuid();

            var result = await _courseService.CreateCourse(courseDto);

            return CheckResult(result);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
        Policy = "AddDeleteEdit_Course")]
        [HttpDelete("Delete")]
        public async Task<IActionResult> DeleteCourse(Guid idd)
        {
            var result = await _courseService.DeleteCourse(idd);

            return CheckResult(result);
        }


        [HttpGet]
        public async Task<IActionResult> GetListOfCourses()
        {
            var result = await _courseService.GetAllCourses();

            return CheckResult(result);
        }

        [HttpGet("{idd}")]
        public async Task<IActionResult> GetCourseByName(Guid idd)
        {
            var result = await _courseService.GetCourse(idd);

            return CheckResult(result);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Policy = "AddDeleteEdit_Course")]
        [HttpPut("{idd}/Update")]
        public async Task<IActionResult> UpdateCourseByName(Guid idd, CourseDto courseDto)
        {
            courseDto.Idd = idd;

            var result = await _courseService.UpdateCourse(courseDto);

            return CheckResult(result);
        }

        [HttpGet("{idd}/Experiments")]
        public async Task<IActionResult> GetExpAssignedToCourse(Guid idd)
        {
            var result = await _courseService.GetExpAssignedToCourse(idd);

            return CheckResult(result);
        }

        [HttpGet("{idd}/Students")]
        public async Task<IActionResult> GetStudentAssignedToCourse(Guid idd)
        {
            var result = await _courseService.GetUsersAssignedToCourse(idd,"student");

            return CheckResult(result);
        }

        [HttpGet("{idd}/Teachers")]
        public async Task<IActionResult> GetTeachersAssignedToCourse(Guid idd)
        {
            var result = await _courseService.GetUsersAssignedToCourse(idd, "teacher");

            return CheckResult(result);
        }

        [HttpGet("{idd}/{role}")]
        public async Task<IActionResult> GetTeachersAssignedToCourse(Guid idd,string role)
        {
            var result = await _courseService.GetUsersAssignedToCourse(idd, role);

            return CheckResult(result);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Policy = "AssignUserToCourse")]
        [HttpPost("{idd}/Assign-Teacher")]
        public async Task<IActionResult> AssignTeacherToCourse(Guid userIdd, Guid courseIdd)
        {
            var result = await _courseService.AssignUserToCourse(userIdd, courseIdd, "teacher");

            return CheckResult(result);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Policy = "AssignUserToCourse")]
        [HttpPost("{idd}/Assign-Students")]
        public async Task<IActionResult> AssignStudentToCourse(List<Guid> userIdds, Guid idd)
        {
            foreach (var userIdd in userIdds)
            {
                await _courseService.AssignUserToCourse(userIdd, idd, "student");
            }
            var result = new Result()
            {
                Status = true,
                Message = "Students Assigned"
            };

            return CheckResult(result);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Policy = "AssignUserToCourse")]
        [HttpPost("{idd}/Assign-User")]
        public async Task<IActionResult> AssignUserToCourse(Guid userIdd, Guid idd, string role)
        {
            var result = await _courseService.AssignUserToCourse(userIdd, idd, role);

            return CheckResult(result);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Policy = "AssignExpToCourse")]
        [HttpPost("{idd}/Assign-Experiment")]
        public async Task<IActionResult> AssignExpToCourse(Guid idd, AssignExpDto assignExpDto)
        {
            var result = await _courseService.AssignExpToCourse(assignExpDto.ExpIdd, idd,
                                                                assignExpDto.StartDate,
                                                                assignExpDto.EndDate,
                                                                assignExpDto.NumberOfTrials);

            return CheckResult(result);
        }

    }

    public class AssignExpDto
    {
        public Guid ExpIdd { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int NumberOfTrials { get; set; }
    }
}
