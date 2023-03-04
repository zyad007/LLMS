using LLS.BLL.IServices;
using LLS.Common.Dto;
using LLS.Common.Models.LLO;
using LLS.Common.Transfere_Layer_Object;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.HttpSys;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace LLS.API.Controllers
{
    
    public class CourseController : BaseController
    {
        private readonly ICourseService _courseService;
        private readonly ITeacherService _teacherService;
        private readonly IStudentService _studentService;
        public CourseController(ICourseService courseService,
            ITeacherService teacherService,
            IStudentService studentService)
        {
            _courseService = courseService;
            _teacherService = teacherService;
            _studentService= studentService;
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Policy = "AddDeleteEdit_Course")]
        [HttpPost]
        public async Task<IActionResult> CreateCourse(CourseDto courseDto)
        {
            courseDto.Idd = Guid.NewGuid();

            var result = await _courseService.CreateCourse(courseDto);

            return CheckResult(result);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
        Policy = "AddDeleteEdit_Course")]
        [HttpDelete("{idd}")]
        public async Task<IActionResult> DeleteCourse(Guid idd)
        {
            var result = await _courseService.DeleteCourse(idd);

            return CheckResult(result);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet]
        public async Task<IActionResult> GetListOfCourses([FromQuery] int page)
        {
            var role = HttpContext.User.FindFirst(ClaimTypes.Role).Value;
            
            if(role.ToLower() == "teacher")
            {
                var teacherId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                var result = await _teacherService.GetCourses(Guid.Parse(teacherId), page -1, false);
                return CheckResult(result);
            }
            else if(role.ToLower() == "student")
            {
                var userEmail = HttpContext.User.FindFirst(ClaimTypes.Email).Value;
                var result = await _studentService.GetStudentCourses(userEmail, page-1);
                return CheckResult(result);
            }
            else
            {
                var result = await _courseService.GetAllCourses(page - 1);
                return CheckResult(result);
            }
        }

        [HttpGet("{idd}")]
        public async Task<IActionResult> GetCourseByName(Guid idd)
        {
            var result = await _courseService.GetCourse(idd);

            return CheckResult(result);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Policy = "AddDeleteEdit_Course")]
        [HttpPut("{idd}")]
        public async Task<IActionResult> UpdateCourseByName(Guid idd, CourseDto courseDto)
        {
            courseDto.Idd = idd;

            var result = await _courseService.UpdateCourse(courseDto);

            return CheckResult(result);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Policy = "AssignUserToCourse")]
        [HttpPost("{idd}/Teacher")]
        public async Task<IActionResult> AssignTeacherToCourse(Guid userIdd, Guid idd)
        {
            var result = await _courseService.AssignUserToCourse(userIdd, idd, "teacher");

            return CheckResult(result);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Policy = "AssignUserToCourse")]
        [HttpPost("{idd}/Students")]
        public async Task<IActionResult> AssignStudentToCourse(StudentsAssign userIdds, Guid idd)
        {
            var list = new List<Result>();
            foreach (var userIdd in userIdds.UserIdds)
            {
                var res = await _courseService.AssignUserToCourse(userIdd, idd, "student");
                list.Add(res);
            }
            
            return Ok(list);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Policy = "AssignUserToCourse")]
        [HttpPost("{idd}/User")]
        public async Task<IActionResult> AssignUserToCourse(Guid userIdd, Guid idd, string role)
        {
            var result = await _courseService.AssignUserToCourse(userIdd, idd, role);

            return CheckResult(result);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Policy = "AssignExpToCourse")]
        [HttpPost("{idd}/Experiment")]
        public async Task<IActionResult> AssignExpToCourse(Guid idd, AssignExpDto assignExpDto)
        {
            var result = await _courseService.AssignExpToCourse(assignExpDto.ExpIdd, idd,
                                                                assignExpDto.StartDate,
                                                                assignExpDto.EndDate,
                                                                assignExpDto.NumberOfTrials);

            return CheckResult(result);
        }

        [HttpGet("{idd}/Students")]
        public async Task<IActionResult> GetStudentAssignedToCourse(Guid idd, [FromQuery] int page)
        {
            var result = await _courseService.GetUsersAssignedToCourse(idd, "student", page - 1);

            return CheckResult(result);
        }

        [HttpGet("{idd}/Teacher")]
        public async Task<IActionResult> GetTeachersAssignedToCourse(Guid idd, [FromQuery] int page)
        {
            var result = await _courseService.GetUsersAssignedToCourse(idd, "teacher", page - 1);

            return CheckResult(result);
        }

        [HttpGet("{idd}/User")]
        public async Task<IActionResult> GetTeachersAssignedToCourse(Guid idd,[FromQuery] string role, [FromQuery] int page)
        {
            var result = await _courseService.GetUsersAssignedToCourse(idd, role, page - 1);

            return CheckResult(result);
        }

        [HttpGet("{idd}/Experiment")]
        public async Task<IActionResult> GetExpAssignedToCourse(Guid idd, [FromQuery] int page)
        {
            var result = await _courseService.GetExpAssignedToCourse(idd, page - 1);

            return CheckResult(result);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Policy = "GetAssignedCourse_Teacher")]
        [HttpGet("{idd}/Experiment/{expIdd}/Grade-Book")]
        public async Task<IActionResult> GetTeacherGradeBooksForExp(Guid idd, Guid expIdd, [FromQuery] int page)
        {
            var teacherIdd = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            //var teacherId = "32a12f48-9e55-4dca-a9aa-c8fdfbab2505";
            var res = await _teacherService.GetGradeBookForExp(Guid.Parse(teacherIdd), idd, expIdd, page - 1);
            return CheckResult(res);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Policy = "GetAssignedCourse_Teacher")]
        [HttpGet("{idd}/Experiment/{expIdd}/Grade-Book/{gradeBookId}/Trial")]
        public async Task<IActionResult> Get(Guid idd, Guid expIdd, Guid gradeBookId, [FromQuery] int page)
        {
            var teacherIdd = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            //var teacherId = "32a12f48-9e55-4dca-a9aa-c8fdfbab2505";
            var res = await _teacherService.getStudentTrials(Guid.Parse(teacherIdd), gradeBookId, expIdd, idd,
                page - 1);
            return CheckResult(res);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Policy = "GetAssignedCourse_Teacher")]
        [HttpGet("{idd}/Experiment/{expIdd}/Grade-Book/{gradeBookId}/Trial/{trialId}")]
        public async Task<IActionResult> GetTrial(Guid idd, Guid expIdd, Guid trialId)
        {
            var teacherIdd = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            //var teacherId = "32a12f48-9e55-4dca-a9aa-c8fdfbab2505";
            var res = await _teacherService.getStudentLRO(Guid.Parse(teacherIdd), expIdd, trialId);
            return CheckResult(res);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Policy = "GetAssignedCourse_Teacher")]
        [HttpPost("{idd}/Experiment/{expIdd}/Grade-Book/{gradeBookId}/Trial/{trialId}")]
        public async Task<IActionResult> GetTrial(Guid idd, Guid expIdd, Guid trialId, [FromBody] LLO lro)
        {
            var teacherIdd = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            //var teacherId = "32a12f48-9e55-4dca-a9aa-c8fdfbab2505";
            var res = await _teacherService.GradeStudentTrial(Guid.Parse(teacherIdd), trialId, lro);
            return CheckResult(res);
        }

    }

    public class AssignExpDto
    {
        public Guid ExpIdd { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int NumberOfTrials { get; set; }
    }

    public class StudentsAssign
    {
        public List<Guid> UserIdds { get; set; }
    }
}
