using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using LLS.Common.Dto;
using LLS.Common.Models;
using LLS.Common.Transfere_Layer_Object;
using LLS.DAL.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LLS.API.Controllers
{
    [Route("api/Active-Lab")]
    [ApiController]
    public class ActiveLabController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ActiveLabController( AppDbContext context )
        {
            _context = context;
        }


        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
           Policy = "GradeStudentAnswers_Teacher")]
        [HttpGet]
        public async Task<IActionResult> getActiveLab([FromQuery] int page, string searchByAcademicYear,
            string searchByExperimentName,
            string searchByCourseName
            )
        {
            string host = HttpContext.Request.Host.Value;

            searchByCourseName += "";
            searchByAcademicYear += "";
            searchByExperimentName += "";

            page--;
            if (page == -1)
            {
                page = 0;
            }

            var userEmail = HttpContext.User.FindFirst(ClaimTypes.Email).Value;
            var teacher = await _context.Users.FirstOrDefaultAsync(x => x.Email == userEmail);
            if (teacher == null)
            {
                return BadRequest(
                    new Result()
                    {
                        Message = "User is not found",
                        Status = false
                    }
                    );
            }


            var trials = _context.Trials
                .Where(x => x.Status == "started"
                    && (x.StartedAt.Day) <= 1
                    //&& x.StudentCourse_ExpCourse.Exp_Course.Course.User_Courses.FirstOrDefault(x=>x.UserId==teacher.Id) != null
                    && (x.StudentCourse_ExpCourse.Status == "Reserved" || x.StudentCourse_ExpCourse.Status == "Assigned")
                    )
                .Select(x => new
                {
                    x.Id,
                    x.StartedAt,
                    x.Status,
                    x.StudentCourse_ExpCourse.Student_Course.User.Email,
                    Name = x.StudentCourse_ExpCourse.Student_Course.User.FirstName + " " + x.StudentCourse_ExpCourse.Student_Course.User.Lastname,
                    x.StudentCourse_ExpCourse.Student_Course.User.AcademicYear,
                    CourseName = x.StudentCourse_ExpCourse.Student_Course.Course.Name,
                    ExperimentName = x.StudentCourse_ExpCourse.Exp_Course.Experiment.Name,

                    progress = new Random().Next(1, 100)
                }).Where(x => x.AcademicYear.ToLower().Contains("" + searchByAcademicYear.ToLower()) &&
                        x.ExperimentName.ToLower().Contains("" + searchByExperimentName.ToLower()) &&
                        x.CourseName.ToLower().Contains("" + searchByCourseName.ToLower())
                ).ToList();

            trials.Insert(0,new
            {
                Id = Guid.NewGuid(),
                StartedAt = DateTime.Now,
                Status = "Started",
                Email = "ali_ahmed@mail.com",
                Name = "Ali Ahmed",
                AcademicYear = "Comm 4",
                CourseName = "",
                ExperimentName = "Python Exam",
                progress = new Random().Next(1, 100)
            });

            var trialsPaged = trials.Skip(page * 10).Take(10).ToList();
            var count = trials.Count;

            return Ok(new Result()
            {
                Status = true,
                Data = new
                {
                    result = trialsPaged,
                    count,
                    next = (page * 10) + 10 >= count ? null : $"https://{host}/api/Active-Lab?page={page + 1 + 1}",
                    previous = page == 0 ? null : $"https://{host}/api/Active-Lab?page={page - 1 + 1}"
                }
            });
        }
    }
}
