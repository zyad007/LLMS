using LLS.BLL.IServices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace LLS.API.Controllers
{
    public class TeacherController : BaseController
    {
        private readonly ITeacherService _teacherService;
        public TeacherController(ITeacherService teacherService)
        {
            _teacherService = teacherService;
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Policy = "GetAssignedCourse_Teacher")]
        [HttpGet("{idd}/Courses")]
        public async Task<IActionResult> GetTeacherCourse(Guid idd)
        {
            var res = await _teacherService.GetTeacherCourses(idd);
            return CheckResult(res);
        }
    }
}
