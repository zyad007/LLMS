using LLS.BLL.IServices;
using LLS.Common.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

        [HttpPost("Create")]
        public async Task<IActionResult> CreateCourse(CourseDto courseDto)
        {
            var result = await _courseService.CreateCourse(courseDto);

            return CheckResult(result);
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> DeleteCourse(string courseName)
        {
            var result = await _courseService.DeleteCourse(courseName);

            return CheckResult(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetListOfCourses()
        {
            var result = await _courseService.GetAllCourses();

            return CheckResult(result);
        }

        [HttpGet("{idd}")]
        public async Task<IActionResult> GetCourseByName(string idd)
        {
            var result = await _courseService.GetCourse(idd);

            return CheckResult(result);
        }

        [HttpGet("{idd}/Experiments")]
        public async Task<IActionResult> GetExpAssignedToCourse(string idd)
        {
            var result = await _courseService.GetExpAssignedToCourse(idd);

            return CheckResult(result);
        }

        [HttpGet("{idd}/Students")]
        public async Task<IActionResult> GetStudentAssignedToCourse(string idd)
        {
            var result = await _courseService.GetUsersAssignedToCourse(idd,"student");

            return CheckResult(result);
        }

        [HttpGet("{idd}/Teachers")]
        public async Task<IActionResult> GetTeachersAssignedToCourse(string idd)
        {
            var result = await _courseService.GetUsersAssignedToCourse(idd, "teacher");

            return CheckResult(result);
        }

        [HttpGet("{idd}/{role}")]
        public async Task<IActionResult> GetTeachersAssignedToCourse(string idd,string role)
        {
            var result = await _courseService.GetUsersAssignedToCourse(idd, role);

            return CheckResult(result);
        }

        [HttpPost("{idd}/Assign-Teacher")]
        public async Task<IActionResult> AssignTeacherToCourse(string email, string idd)
        {
            var result = await _courseService.AssignUserToCourse(email, idd,"teacher");

            return CheckResult(result);
        }

        [HttpPost("{idd}/Assign-Student")]
        public async Task<IActionResult> AssignStudentToCourse(string email, string idd)
        {
            var result = await _courseService.AssignUserToCourse(email, idd, "student");

            return CheckResult(result);
        }

        [HttpPost("{idd}/Assign-User")]
        public async Task<IActionResult> AssignUserToCourse(string email, string idd, string role)
        {
            var result = await _courseService.AssignUserToCourse(email, idd, role);

            return CheckResult(result);
        }

        [HttpPost("{idd}/Assign-Experiment")]
        public async Task<IActionResult> AssignExpToCourse(string idd, AssignExpDto assignExpDto)
        {
            var result = await _courseService.AssignExpToCourse(assignExpDto.ExpIdd, idd,
                                                                assignExpDto.StartDate,
                                                                assignExpDto.EndDate,
                                                                assignExpDto.NumberOfTrials,
                                                                assignExpDto.ResourceIds);

            return CheckResult(result);
        }

    }

    public class AssignExpDto
    {
        public Guid ExpIdd { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int NumberOfTrials { get; set; }
        public List<Guid> ResourceIds { get; set; }
    }
}
