//using LLS.BLL.IServices;
//using LLS.Common.Models.LLO;
//using Microsoft.AspNetCore.Authentication.JwtBearer;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using System;
//using System.Security.Claims;
//using System.Threading.Tasks;

//namespace LLS.API.Controllers
//{
//    public class TeacherController : BaseController
//    {
//        private readonly ITeacherService _teacherService;
//        public TeacherController(ITeacherService teacherService)
//        {
//            _teacherService = teacherService;
//        }

//        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
//            Policy = "GetAssignedCourse_Teacher")]
//        [HttpGet("Courses")]
//        public async Task<IActionResult> GetTeacherCourse([FromQuery] int page)
//        {
//            var teacherIdd = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
//            var res = await _teacherService.GetCourses(Guid.Parse(teacherIdd), page-1, true);
//            return CheckResult(res);
//        }

//        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
//            Policy = "GetAssignedCourse_Teacher")]
//        [HttpGet("Courses/{courseIdd}/Experiments")]
//        public async Task<IActionResult> GetTeacherExps(Guid courseIdd, [FromQuery] int page)
//        {
//            var teacherIdd = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
//            //var teacherId = "32a12f48-9e55-4dca-a9aa-c8fdfbab2505";
//            var res = await _teacherService.getExps(Guid.Parse(teacherIdd), courseIdd, page-1);
//            return CheckResult(res);
//        }

//        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
//            Policy = "GetAssignedCourse_Teacher")]
//        [HttpGet("Courses/{courseIdd}/Experiments/{expIdd}/Grade-Books")]
//        public async Task<IActionResult> GetTeacherGradeBooksForExp(Guid courseIdd, Guid expIdd,[FromQuery] int page)
//        {
//            var teacherIdd = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
//            //var teacherId = "32a12f48-9e55-4dca-a9aa-c8fdfbab2505";
//            var res = await _teacherService.GetGradeBookForExp(Guid.Parse(teacherIdd), courseIdd, expIdd, page-1);
//            return CheckResult(res);
//        }

//        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
//            Policy = "GetAssignedCourse_Teacher")]
//        [HttpGet("Courses/{courseIdd}/Experiments/{expIdd}/Grade-Books/{gradeBookId}/Trials")]
//        public async Task<IActionResult> Get(Guid courseIdd, Guid expIdd, Guid gradeBookId, [FromQuery] int page)
//        {
//            var teacherIdd = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
//            //var teacherId = "32a12f48-9e55-4dca-a9aa-c8fdfbab2505";
//            var res = await _teacherService.getStudentTrials(Guid.Parse(teacherIdd), gradeBookId,expIdd,courseIdd,
//                page-1);
//            return CheckResult(res);
//        }

//        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
//            Policy = "GetAssignedCourse_Teacher")]
//        [HttpGet("Courses/{courseIdd}/Experiments/{expIdd}/Grade-Books/{gradeBookId}/Trials/{trialId}")]
//        public async Task<IActionResult> GetTrial(Guid courseIdd, Guid expIdd, Guid trialId)
//        {
//            var teacherIdd = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
//            //var teacherId = "32a12f48-9e55-4dca-a9aa-c8fdfbab2505";
//            var res = await _teacherService.getStudentLRO(Guid.Parse(teacherIdd),expIdd, trialId);
//            return CheckResult(res);
//        }

//        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
//            Policy = "GetAssignedCourse_Teacher")]
//        [HttpPost("Courses/{courseIdd}/Experiments/{expIdd}/Grade-Books/{gradeBookId}/Trials/{trialId}/Grade")]
//        public async Task<IActionResult> GetTrial(Guid courseIdd, Guid expIdd, Guid trialId,[FromBody] LLO lro)
//        {
//            var teacherIdd = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
//            //var teacherId = "32a12f48-9e55-4dca-a9aa-c8fdfbab2505";
//            var res = await _teacherService.GradeStudentTrial(Guid.Parse(teacherIdd), trialId, lro);
//            return CheckResult(res);
//        }
//    }
//}
