using LLS.BLL.IServices;
using LLS.Common.Dto;
using LLS.Common.Models.LLO;
using LLS.Common.Transfere_Layer_Object;
using LLS.DAL.Data;
using MailKit.Net.Imap;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using static LLS.API.Controllers.StudentController;
using static LLS.BLL.Services.StudentService;

namespace LLS.API.Controllers
{
    public class StudentController : BaseController
    {
        private readonly IStudentService _sudentService;
        private readonly AppDbContext _context;
        public StudentController(IStudentService sudentService, AppDbContext context)
        {
            _sudentService = sudentService;
            _context = context;
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Policy = "ViewAssignedExpCourse_Student")]
        [HttpGet("Assigned")]
        public async Task<IActionResult> GetAllExpAssignedToStudent([FromQuery] int page, string searchByExperimentName, string searchByCourseName)
        {
            var userEmail = HttpContext.User.FindFirst(ClaimTypes.Email).Value;

            var res = await _sudentService.GetAssignedExpForStudent(userEmail, page - 1, searchByExperimentName, searchByCourseName);

            return CheckResult(res);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Policy = "ViewAssignedExpCourse_Student")]
        [HttpGet("Assigned/{idd}")]
        public async Task<IActionResult> GetExp(Guid idd)
        {
            var exp = await _context.Expirments.FirstOrDefaultAsync(x => x.Idd == idd);
            if (exp == null)
                return BadRequest(new Result()
                {
                    Message = "there is no Experiment with this IDD",
                    Status = false
                });

            var exp_course = await _context.Exp_Courses.FirstOrDefaultAsync(x => x.ExperimentId == exp.Id);
            if (exp_course == null)
                return BadRequest(new Result()
                {
                    Message = "Some thing went wrong",
                    Status = false
                });


            return Ok(new Result()
            {
                Data = new ExpDto()
                {
                    Idd = exp.Idd,
                    Name = exp.Name,
                    Description = exp.Description,
                    StartDate = exp_course.StartDate,
                    EndDate = exp_course.EndDate
                },
                Status = true
            });
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Policy = "ViewAssignedExpCourse_Student")]
        [HttpPost("Assigned/{idd}/Get-Time-Slot")]
        public async Task<IActionResult> GetAvvailableTimeSlot(Guid idd,GetAvailableTime getAvailableTime)
        {
            getAvailableTime.day.AddHours(2);
            var userEmail = HttpContext.User.FindFirst(ClaimTypes.Email).Value;
            var student = await _context.Users.FirstOrDefaultAsync(x => x.Email == userEmail);
            if (student == null)
            {
                return BadRequest(new Result()
                {
                    Message = "User doesn't exist",
                    Status = false
                });
            }

            var exp = await _context.Expirments.FirstOrDefaultAsync(x => x.Idd == idd);
            if (exp == null)
            {
                return BadRequest(new Result()
                {
                    Message = "Exp doesn't exist",
                    Status = false
                });
            }

            var exp_course = await _context.Exp_Courses.FirstOrDefaultAsync(x => x.ExperimentId == exp.Id);
            if (exp_course == null)
            {
                return BadRequest(new Result()
                {
                    Message = "Course doesn't exist",
                    Status = false
                });
            }

            if (exp_course.StartDate <= getAvailableTime.day.AddDays(1) && exp_course.EndDate.AddDays(1) >= getAvailableTime.day)
            {
                DateTime start = new DateTime(2001,5,6,9,0,0);
                DateTime end = start.AddHours(2);

                List<Reservication> timeSlots = new List<Reservication>();
                for (int i = 0; i < 6; i++)
                {
                    timeSlots.Add(new Reservication()
                    {
                        slotId = i + 1,
                        reserved = false,
                        start = start.ToShortTimeString(),
                        end = end.ToShortTimeString()
                    });
                    start = start.AddHours(2);
                    end = end.AddHours(2);
                }

                var studentExpCourses = _context.StudentCourse_ExpCourses.Where(x => x.ReservedDay == getAvailableTime.day && x.Status == "Reserved").ToList();

                foreach (var studentExpCourse in studentExpCourses)
                {
                    timeSlots[studentExpCourse.TimeSlot - 1].reserved = true;
                }

                return Ok(new Result()
                {
                    Data = timeSlots,
                    Status = true
                });
            }
            else
            {
                return BadRequest(new Result()
                {
                    Message = "Selected day is out of assigned boundry time",
                    Status = false
                });
            }
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Policy = "ViewAssignedExpCourse_Student")]
        [HttpPost("Assigned/{idd}/Time-Slot")]
        public async Task<IActionResult> reserveTimeSlot(Guid idd,ReserveAvailableTime reserve)
        {
            reserve.day.AddHours(2);

            if (reserve.slotId == 0)
            {
                return BadRequest(new Result()
                {
                    Message = "Invalid Time Slot",
                    Status = false
                });
            }

            var userEmail = HttpContext.User.FindFirst(ClaimTypes.Email).Value;
            var student = await _context.Users.FirstOrDefaultAsync(x => x.Email == userEmail);
            if (student == null)
            {
                return BadRequest(new Result()
                {
                    Message = "User doesn't exist",
                    Status = false
                });
            }

            var exp = await _context.Expirments.FirstOrDefaultAsync(x => x.Idd == idd);
            if (exp == null)
            {
                return BadRequest(new Result()
                {
                    Message = "Exp doesn't exist",
                    Status = false
                });
            }

            var course = await _context.Courses.FirstOrDefaultAsync(x => x.Exp_Courses.FirstOrDefault(x => x.ExperimentId == exp.Id) != null);
            if (course == null)
            {
                return BadRequest(new Result()
                {
                    Message = "Course doesn't exist",
                    Status = false
                });
            }

            var studentExpCourse = await _context.StudentCourse_ExpCourses
    .FirstOrDefaultAsync(x => x.Exp_Course.ExperimentId == exp.Id && x.Student_Course.UserId == student.Id);

            var studentExpCoursExists = await _context.StudentCourse_ExpCourses
                .FirstOrDefaultAsync(x => x.Id != studentExpCourse.Id && x.Status == "Reserved" && x.ReservedDay == reserve.day && x.TimeSlot == reserve.slotId);
            if (studentExpCoursExists != null)
            {
                return BadRequest(new Result()
                {
                    Message = "This time  slot is already reserved",
                    Status = false
                });
            }



            studentExpCourse.Status = "Reserved";
            studentExpCourse.ReservedDay = reserve.day;
            studentExpCourse.TimeSlot = reserve.slotId;
            studentExpCourse.StartFrom = reserve.day.AddHours(9 + (reserve.slotId - 1) * 2);
            studentExpCourse.EndAt = studentExpCourse.StartFrom.AddHours(2);

            _context.SaveChanges();

            return Ok(new Result()
            {
                Message = "Reserved successfully",
                Status = true
            });

        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Policy = "SubmitAssignedExp_Student")]
        [HttpPost("Start-Trial")]
        public async Task<IActionResult> StartTrial(Guid idd)
        {
            var studentIdId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var res = await _sudentService.startTrial(idd, studentIdId);

            return CheckResult(res);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Policy = "SubmitAssignedExp_Student")]
        [HttpPost("Submit-Trial")]
        public async Task<IActionResult> SubmitTrial(TrialSubmit studentSubmit)
        {
            var studentIdId = HttpContext.User.FindFirst(ClaimTypes.Email).Value;

            var res = await _sudentService.SubmitTrial(studentSubmit, studentIdId);

            return CheckResult(res);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Policy = "ViewAssignedExpCourse_Student")]
        [HttpGet("Completed")]
        public async Task<IActionResult> GetAllCompletedExp([FromQuery] int page, string searchByExperimentName, string searchByCourseName)
        {
            var userEmail = HttpContext.User.FindFirst(ClaimTypes.Email).Value;

            var res = await _sudentService.GetCompletedExp(userEmail, page - 1, searchByExperimentName, searchByCourseName);

            return CheckResult(res);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
           Policy = "ViewAssignedExpCourse_Student")]
        [HttpGet("Completed/{idd}")]
        public async Task<IActionResult> GetExpTrial(Guid idd)
        {
            var userEmail = HttpContext.User.FindFirst(ClaimTypes.Email).Value;
            var student = await _context.Users.FirstOrDefaultAsync(x => x.Email == userEmail);
            if (student == null)
            {
                return BadRequest(new Result()
                {
                    Message = "User doesn't exist",
                    Status = false
                });
            }

            var exp = await _context.Expirments.FirstOrDefaultAsync(x => x.Idd == idd);
            if (exp == null)
            {
                return BadRequest(new Result()
                {
                    Message = "Exp doesn't exist",
                    Status = false
                });
            }

            var trials = _context.StudentCourse_ExpCourses
                .Where(x => x.Exp_Course.ExperimentId == exp.Id && x.Student_Course.UserId == student.Id)
                .Select(x=> x.Trials.Where(x=>x.IsGraded));

            //var trialsDebug = trials.FirstOrDefault();

            var trial = trials.FirstOrDefault().LastOrDefault();

            if (trial == null)
            {
                var studentCourse_ExpCourses = _context.StudentCourse_ExpCourses
                .Where(x => x.Exp_Course.ExperimentId == exp.Id && x.Student_Course.UserId == student.Id)
                .FirstOrDefault();

                var trialNotGraded = _context.Trials
                    .Where(x => x.StudentCourse_ExpCourseId == studentCourse_ExpCourses.Id)
                    .FirstOrDefault();


                //var trialNotGraded = trials.FirstOrDefault().LastOrDefault();


                return Ok(new Result()
                {
                    Data = new
                    {
                        LLO = JsonConvert.DeserializeObject<LLO>(exp.LLO),
                        LRO = JsonConvert.DeserializeObject<LLO>(trialNotGraded.LLA)
                    },
                    Status = true
                });
            }

            return Ok(new Result()
            {
                Data = new
                {
                    LLO = JsonConvert.DeserializeObject<LLO>(exp.LLO),
                    LRO = JsonConvert.DeserializeObject<LLO>(trial.LRO)
                },
                Status = true
            });


        }


        public class GetAvailableTime
        {
            public DateTime day { get; set; }
        }

        public class ReserveAvailableTime
        {
            public DateTime day { get; set; }
            public int slotId { get; set; }
        }

        public class Reservication
        {
            public int slotId { get; set; }
            public bool reserved { get; set; }
            public string start { get; set; }
            public string end { get; set; }
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
