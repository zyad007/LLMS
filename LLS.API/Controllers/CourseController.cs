using LLS.BLL.IServices;
using LLS.Common.Dto;
using LLS.Common.Models.LLO;
using LLS.Common.Models.LLO.Pages;
using LLS.Common.Transfere_Layer_Object;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.HttpSys;
using Microsoft.Net.Http.Headers;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.Grid;
using Syncfusion.Pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;
using LLS.DAL.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Newtonsoft.Json;
using LLS.Common.Models;

namespace LLS.API.Controllers
{
    
    public class CourseController : BaseController
    {
        private readonly ICourseService _courseService;
        private readonly ITeacherService _teacherService;
        private readonly IStudentService _studentService;
        private static IWebHostEnvironment _enviroment;
        private readonly AppDbContext _context;
        public CourseController(ICourseService courseService,
            ITeacherService teacherService,
            IStudentService studentService,
            IWebHostEnvironment enviroment,
            AppDbContext context)
        {
            _courseService = courseService;
            _teacherService = teacherService;
            _studentService= studentService;
            _enviroment = enviroment;
            _context = context;
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
        public async Task<IActionResult> GetListOfCourses([FromQuery] int page, string searchByCourseName, string searchByCourseCode)
        {
            var role = HttpContext.User.FindFirst(ClaimTypes.Role).Value;
            
            if(role.ToLower() == "teacher")
            {
                var teacherId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                var result = await _teacherService.GetCourses(Guid.Parse(teacherId), page -1, false, searchByCourseName, searchByCourseCode);
                return CheckResult(result);
            }
            else if(role.ToLower() == "student")
            {
                var userEmail = HttpContext.User.FindFirst(ClaimTypes.Email).Value;
                var result = await _studentService.GetStudentCourses(userEmail, page-1, searchByCourseName, searchByCourseCode);
                return CheckResult(result);
            }
            else
            {
                var result = await _courseService.GetAllCourses(page - 1, searchByCourseName, searchByCourseCode);
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
        public async Task<IActionResult> GetStudentAssignedToCourse(Guid idd, [FromQuery] int page
            , string searchByEmail)
        {
            var result = await _courseService.GetUsersAssignedToCourse(idd, "student", page - 1, searchByEmail);

            return CheckResult(result);
        }

        [HttpGet("{idd}/Teacher")]
        public async Task<IActionResult> GetTeachersAssignedToCourse(Guid idd, [FromQuery] int page,
            string searchByEmail)
        {
            var result = await _courseService.GetUsersAssignedToCourse(idd, "teacher", page - 1, searchByEmail);

            return CheckResult(result);
        }

        [HttpGet("{idd}/User")]
        public async Task<IActionResult> GetTeachersAssignedToCourse(Guid idd,[FromQuery] string role, [FromQuery] int page, string searchByEmail)
        {
            var result = await _courseService.GetUsersAssignedToCourse(idd, role, page - 1, searchByEmail);

            return CheckResult(result);
        }

        [HttpGet("{idd}/Experiment")]
        public async Task<IActionResult> GetExpAssignedToCourse(Guid idd, [FromQuery] int page, string searchByExperimentName)
        {
            var result = await _courseService.GetExpAssignedToCourse(idd, page - 1, searchByExperimentName);

            return CheckResult(result);
        }

        

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Policy = "GradeStudentAnswers_Teacher")]
        [HttpGet("{idd}/Experiment/{expIdd}/Grade-Book")]
        public async Task<IActionResult> GetTeacherGradeBooksForExp(Guid idd, Guid expIdd, [FromQuery] int page, string searchByEmail)
        {
            var teacherIdd = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            //var teacherId = "32a12f48-9e55-4dca-a9aa-c8fdfbab2505";
            var res = await _teacherService.GetGradeBookForExp(Guid.Parse(teacherIdd), idd, expIdd, page - 1, searchByEmail);
            return CheckResult(res);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Policy = "GradeStudentAnswers_Teacher")]
        [HttpGet("{idd}/Experiment/{expIdd}/Grade-Book/Report")]
        public async Task<IActionResult> getReport(Guid idd, Guid expIdd)
        {
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

            var course = await _context.Courses.FirstOrDefaultAsync(x=>x.Idd == idd);
            if (course == null)
            {
                return BadRequest(
                    new Result()
                    {
                        Message = "Course is not found",
                        Status = false
                    }
                    );
            }

            var exp = await _context.Expirments.FirstOrDefaultAsync(x=>x.Idd == expIdd);
            if(exp == null)
            {
                return BadRequest(
                    new Result()
                    {
                        Message = "Experiment is not found",
                        Status = false
                    }
                    );
            }

            PdfDocument document = new PdfDocument();

            PdfPage page = document.Pages.Add();

            PdfGraphics graphics = page.Graphics;

            FileStream imageStream = new FileStream("Logo/2.jpeg", FileMode.Open, FileAccess.Read);
            PdfBitmap image = new PdfBitmap(imageStream);
            //Draw the image
            graphics.DrawImage(image, 400, 0);

            //Set the standard font.
            PdfFont font = new PdfStandardFont(PdfFontFamily.TimesRoman, 12);
            PdfFont fontBold = new PdfStandardFont(PdfFontFamily.Helvetica, 12, PdfFontStyle.Bold);
            //Draw the text.
            graphics.DrawString($"Course ID: {course.Idd}", font, PdfBrushes.Black, new Syncfusion.Drawing.PointF(0, 0));
            graphics.DrawString($"Course Code: {course.Code}", font, PdfBrushes.Black, new Syncfusion.Drawing.PointF(0, 20));
            graphics.DrawString($"Course Name: {course.Name}", font, PdfBrushes.Black, new Syncfusion.Drawing.PointF(0, 40));
            graphics.DrawString($"Experiment Name: {exp.Name}", font, PdfBrushes.Black, new Syncfusion.Drawing.PointF(0, 60));

            var expCourse = await _context.Exp_Courses.FirstOrDefaultAsync(x => x.CourseId == course.Id && x.ExperimentId == exp.Id);


            PdfGrid pdfGrid = new PdfGrid();

            PdfGridLayoutResult pdfGridLayoutResult;
            List<object> data = new List<object>();

            var studentsExpCourse = await _context.StudentCourse_ExpCourses
                .Where(x => x.Exp_CourseId == expCourse.Id).ToListAsync();

            studentsExpCourse.ForEach(x =>
            {
                var user =  _context.User_Courses.Where(y => y.Id == x.Student_CourseId)
                .Select(x => x.User).FirstOrDefault();

                Object row1 = new
                {
                    Student_ID = user.Id,
                    Student_Name = user.FirstName + " " + user.Lastname,
                    Degree = x.FinalGrade
                };

                data.Add(row1);
            });

            IEnumerable<object> dataTable = data;
            //Assign data source.
            pdfGrid.DataSource = dataTable;
            //Draw grid to the page of PDF document.
            pdfGridLayoutResult = pdfGrid.Draw(page, new Syncfusion.Drawing.PointF(0, 140));
            //pdfGrid.Draw(page, new Syncfusion.Drawing.PointF(0, pdfGridLayoutResult.Bounds.Bottom + 20));
            

            Stream stream = new MemoryStream();

            document.Save(stream);

            stream.Position = 0;

            var url = createFile(stream, $"Exp-GradeBook-{expIdd}");

            return Ok(new { url });
        }

        private string createFile(Stream stream, string userIdd)
        {
            //File
            string host = HttpContext.Request.Host.Value;

            if (!Directory.Exists(_enviroment.WebRootPath + "/Reports/"))
            {
                Directory.CreateDirectory(_enviroment.WebRootPath + "/Reports/");
            }
            string baseUrl = _enviroment.WebRootPath + "/Reports/";

            var fileName = $"Report-{"" + userIdd}.pdf";

            using (var fileStream = System.IO.File.Create(baseUrl + fileName))
            {
                CopyStream(stream, fileStream);
            }

            return "https://" + host + $"/Reports/{fileName}";
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Policy = "GradeStudentAnswers_Teacher")]
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
            Policy = "GradeStudentAnswers_Teacher")]
        [HttpGet("{idd}/Experiment/{expIdd}/Grade-Book/{gradeBookId}/Report")]
        public async Task<IActionResult> getReport(Guid idd, Guid expIdd, Guid gradeBookId)
        {
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

            var course = await _context.Courses.FirstOrDefaultAsync(x => x.Idd == idd);
            if (course == null)
            {
                return BadRequest(
                    new Result()
                    {
                        Message = "Course is not found",
                        Status = false
                    }
                    );
            }

            var exp = await _context.Expirments.FirstOrDefaultAsync(x => x.Idd == expIdd);
            if (exp == null)
            {
                return BadRequest(
                    new Result()
                    {
                        Message = "Experiment is not found",
                        Status = false
                    }
                    );
            }

            var studentExpCourse = await _context.StudentCourse_ExpCourses.FirstOrDefaultAsync(x => x.Id == gradeBookId);
            if (studentExpCourse == null)
            {
                return BadRequest(
                    new Result()
                    {
                        Message = "GradeBook is not found",
                        Status = false
                    }
                    );
            }

            var student = await _context.StudentCourse_ExpCourses.Where(x => x.Id == studentExpCourse.Id)
                .Select(x => x.Student_Course.User).FirstOrDefaultAsync();

            var trials = await _context.StudentCourse_ExpCourses.Where(x => x.Id == studentExpCourse.Id)
                .Select(x => x.Trials).FirstOrDefaultAsync();
            PdfDocument document = new PdfDocument();

            int i = 1;
            int qNum = 1;
            float sum = 0;

            foreach(var trial in trials)
            {

                PdfPage page = document.Pages.Add();

                PdfGraphics graphics = page.Graphics;

                FileStream imageStream = new FileStream("Logo/2.jpeg", FileMode.Open, FileAccess.Read);
                PdfBitmap image = new PdfBitmap(imageStream);
                //Draw the image
                graphics.DrawImage(image, 400, 0);

                //Set the standard font.
                PdfFont font = new PdfStandardFont(PdfFontFamily.TimesRoman, 12);
                PdfFont fontBold = new PdfStandardFont(PdfFontFamily.Helvetica, 12, PdfFontStyle.Bold);
                //Draw the text.
                graphics.DrawString($"Student ID: {student.Idd}", font, PdfBrushes.Black, new Syncfusion.Drawing.PointF(0, 0));
                graphics.DrawString($"Student Name: {student.FirstName + " " + student.Lastname}", font, PdfBrushes.Black, new Syncfusion.Drawing.PointF(0, 20));
                graphics.DrawString($"Course Name: {course.Name}", font, PdfBrushes.Black, new Syncfusion.Drawing.PointF(0, 40));
                graphics.DrawString($"Experiment Name: {exp.Name}", font, PdfBrushes.Black, new Syncfusion.Drawing.PointF(0, 60));
                graphics.DrawString($"Trial No. {i}", fontBold, PdfBrushes.Black, new Syncfusion.Drawing.PointF(0, 80));


                var expCourse = await _context.Exp_Courses.FirstOrDefaultAsync(x => x.CourseId == course.Id && x.ExperimentId == exp.Id);


                PdfGrid pdfGrid = new PdfGrid();

                PdfGridLayoutResult pdfGridLayoutResult;
                List<object> data = new List<object>();

                if(trial.LRO != null)
                {
                    LLO lro = JsonConvert.DeserializeObject<LLO>(trial.LRO);

                    foreach (var section in lro.Sections)
                    {
                        foreach (var q in section.Column1)
                        {
                            Object row = new
                            {
                                Question_No = qNum,
                                Degree = q.StudentScore
                            };
                            qNum++;
                            sum += q.StudentScore;
                            data.Add(row);
                        }
                        foreach (var q in section.Column2)
                        {
                            Object row2 = new
                            {
                                Question_No = qNum,
                                Degree = q.StudentScore
                            };
                            qNum++;
                            sum += q.StudentScore;
                            data.Add(row2);
                        }
                        foreach (var q in section.Column3)
                        {
                            Object row3 = new
                            {
                                Question_No = qNum,
                                Degree = q.StudentScore
                            };
                            qNum++;
                            sum += q.StudentScore;
                            data.Add(row3);
                        }
                    }
                }

                Object row1 = new
                {
                    Question_No = "Overall grade",
                    Degree = sum
                };
                data.Add(row1);


                IEnumerable<object> dataTable = data;
                //Assign data source.
                pdfGrid.DataSource = dataTable;
                //Draw grid to the page of PDF document.
                pdfGridLayoutResult = pdfGrid.Draw(page, new Syncfusion.Drawing.PointF(0, 120));
                //pdfGrid.Draw(page, new Syncfusion.Drawing.PointF(0, pdfGridLayoutResult.Bounds.Bottom + 20));
                i++;
            }

            Stream stream = new MemoryStream();

            document.Save(stream);

            stream.Position = 0;

            var url = createFile(stream, $"{student.FirstName}-GradeBook-{expIdd}");

            return Ok(new { url });
        }

        public static void CopyStream(Stream input, Stream output)
        {
            byte[] buffer = new byte[8 * 1024];
            int len;
            while ((len = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                output.Write(buffer, 0, len);
            }
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Policy = "GradeStudentAnswers_Teacher")]
        [HttpGet("{idd}/Experiment/{expIdd}/Grade-Book/{gradeBookId}/Report-Test")]
        public async Task<FileStreamResult> getReportTest(Guid idd, Guid expIdd, Guid gradeBookId)
        {
            var stream = GenerateStreamFromString("test");

            return new FileStreamResult(stream, "application/pdf")
            {
                FileDownloadName = $"Report.pdf"
            };
        }

        public static Stream GenerateStreamFromString(string s)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Policy = "GradeStudentAnswers_Teacher")]
        [HttpGet("{idd}/Experiment/{expIdd}/Grade-Book/{gradeBookId}/Trial/{trialId}")]
        public async Task<IActionResult> GetTrial(Guid idd, Guid expIdd, Guid trialId)
        {
            var teacherIdd = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            //var teacherId = "32a12f48-9e55-4dca-a9aa-c8fdfbab2505";
            var res = await _teacherService.getStudentLRO(Guid.Parse(teacherIdd), expIdd, trialId);
            return CheckResult(res);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Policy = "GradeStudentAnswers_Teacher")]
        [HttpPost("{idd}/Experiment/{expIdd}/Grade-Book/{gradeBookId}/Trial/{trialId}")]
        public async Task<IActionResult> GetTrial(Guid idd, Guid expIdd, Guid trialId,sss req)
        {
            var teacherIdd = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            //var teacherId = "32a12f48-9e55-4dca-a9aa-c8fdfbab2505";
            var res = await _teacherService.GradeStudentTrial(Guid.Parse(teacherIdd), trialId, new LLO() { Sections = req.sections}, req.feedBack);
            return CheckResult(res);
        }

    }

    public class sss
    {
        public List<Section> sections { get; set; }
        public string feedBack { get; set; }
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
