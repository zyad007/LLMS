using LLS.BLL.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

        [HttpGet("Courses")]
        public async Task<IActionResult> GetTeacherCourse(string email)
        {
            var res = await _teacherService.GetTeacherCourses(email);
            return CheckResult(res);
        }
    }
}
