using LLS.Common.Transfere_Layer_Object;
using System.Security.Claims;
using LLS.DAL.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;
using LLS.Common.Dto;
using System;
using AutoMapper.Internal;
using System.Collections.Generic;
using LLS.Common.Models.LLO;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using AutoMapper;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Drawing;
using Syncfusion.Pdf.Grid;

namespace LLS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnalyticsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private static IWebHostEnvironment _enviroment;
        public AnalyticsController(AppDbContext context, IMapper mapper, IWebHostEnvironment envieroment)
        {
            _context = context;
            _mapper = mapper;
            _enviroment = envieroment;
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
           Policy = "ViewAnalytics_Teacher")]
        [HttpGet("Experiment")]
        public async Task<IActionResult> Get(int page, string searchByExperimentName, string searchByRelatedCourse)
        {
            searchByExperimentName += "";
            searchByRelatedCourse += "";

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
                        Message = "User is not teacher",
                        Status = false
                    }
                    );
            }


            var exps = await _context.Exp_Courses
                .Where( x =>
                x.Course.User_Courses.FirstOrDefault(x => x.UserId == teacher.Id) != null)
                .Select(x=> new
                {
                    x.Experiment.Idd,
                    x.Experiment.Name,
                    x.Experiment.RelatedCourse,
                    courseName = x.Course.Name,
                    numberOfStudent = x.StudentCourse_ExpCourses.Count(),
                    x.Experiment.AuthorName,
                    x.StartDate,
                    x.EndDate
                })
                .Where(x=>x.Name.ToLower().Contains(""+ searchByExperimentName.ToLower())).ToListAsync();
            
            var count = exps.Count;
            var expsPaged = exps.Skip(page * 10).Take(10).ToList();

            return Ok(new Result()
            {
                Status = true,
                Data = new
                {
                    result = expsPaged,
                    count,
                    next = (page * 10) + 10 >= count ? null : $"https://optimistic-burnell.74-50-88-98.plesk.page/api/Analytics/Experiment?page={page + 1 + 1}",
                    previous = page == 0 ? null : $"https://optimistic-burnell.74-50-88-98.plesk.page/api/Analytics/Experiment?page={page - 1 + 1}"
                }
            });

        }

        [HttpGet("Experiment/ERO")]
        public async Task<IActionResult> getERO(Guid expIdd)
        {
            var exp = await _context.Expirments.FirstOrDefaultAsync(x => x.Idd == expIdd);
            if(exp == null)
            {
                return BadRequest(new Result()
                {
                    Status = false,
                    Message = "Exp not found"
                });
            }

            if(!exp.hasLLO)
            {
                return BadRequest(new Result()
                {
                    Status = false,
                    Message = "Exp has no LLO"
                });
            }

            var lros = new List<LLO>();

            var trials = _context.Exp_Courses.Where(x => x.ExperimentId == exp.Id)
                .Select(x => x.StudentCourse_ExpCourses.Select(x=>x.Trials))
                .First();

            if(!trials.Any()) 
            {
                return BadRequest(new Result()
                {
                    Status = false,
                    Message = "no trials yet"
                });
            }

            foreach (var x in trials)
            {
                foreach (var y in x)
                {
                    if (y.IsGraded) lros.Add(JsonConvert.DeserializeObject<LLO>(y.LRO));
                }
            }

            var ero = JsonConvert.DeserializeObject<LLO>(exp.LLO);

            for (int sec= 0; sec < ero.Sections.Count; sec ++)
            {
                for (int q = 0; q < ero.Sections[sec].Column1.Count; q++)
                {
                    if (ero.Sections[sec].Column1[q].MainType == Common.Enums.MainType.QUESTION)
                    {
                        foreach(var lro in lros)
                        {
                            ero.Sections[sec].Column1[q].avg += lro.Sections[sec].Column1.Where(x=>x.index == q).First().StudentScore;
                        }
                    }
                }

                for (int q = 0; q < ero.Sections[sec].Column2.Count; q++)
                {
                    if (ero.Sections[sec].Column2[q].MainType == Common.Enums.MainType.QUESTION)
                    {
                        foreach (var lro in lros)
                        {
                            ero.Sections[sec].Column2[q].avg += lro.Sections[sec].Column2.Where(x => x.index == q).First().StudentScore;
                        }
                    }
                }

                for (int q = 0; q < ero.Sections[sec].Column3.Count; q++)
                {
                    if (ero.Sections[sec].Column3[q].MainType == Common.Enums.MainType.QUESTION)
                    {
                        foreach (var lro in lros)
                        {
                            ero.Sections[sec].Column3[q].avg += lro.Sections[sec].Column3.Where(x => x.index == q).First().StudentScore;
                        }
                    }
                }
            }

            foreach(var sec in ero.Sections)
            {
                foreach(var q  in sec.Column1)
                {
                    if (lros.Count != 0) q.avg /= lros.Count;
                }

                foreach (var q in sec.Column2)
                {
                    if(lros.Count != 0) q.avg /= lros.Count;
                }

                foreach (var q in sec.Column3)
                {
                    if (lros.Count != 0) q.avg /= lros.Count;
                }
            }

            return Ok(ero);
        }

        [HttpGet("Experiment/Report")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
           Policy = "ViewAnalytics_Teacher")]
        public async Task<IActionResult> getExpReport(string expIdd)
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

            var courses = _context.User_Courses.Where(x => x.UserId == teacher.Id)
                .Select(x => x.Course).ToList();

            PdfDocument document = new PdfDocument();
            
            foreach(var course in courses)
            {
                PdfPage page = document.Pages.Add();

                PdfGraphics graphics = page.Graphics;

                //Set the standard font.
                PdfFont font = new PdfStandardFont(PdfFontFamily.TimesRoman, 12);
                PdfFont fontBold = new PdfStandardFont(PdfFontFamily.Helvetica, 12, PdfFontStyle.Bold);
                //Draw the text.
                graphics.DrawString($"Course Code: {course.Code}", font, PdfBrushes.Black, new Syncfusion.Drawing.PointF(0, 0));
                graphics.DrawString($"Course Name: {course.Name}", font, PdfBrushes.Black, new Syncfusion.Drawing.PointF(0, 20));
                graphics.DrawString($"Number of Experiments in course: {course.NumberOfExp}", font, PdfBrushes.Black, new Syncfusion.Drawing.PointF(0, 40));

                var expCourses = await _context.Exp_Courses.Where(x => x.CourseId == course.Id).ToListAsync();

                PdfGrid pdfGrid = new PdfGrid();

                PdfGridLayoutResult pdfGridLayoutResult;
                List<object> data = new List<object>();

                foreach(var expCourse in expCourses)
                {
                    var exp = await _context.Expirments.FindAsync(expCourse.ExperimentId);
                    var studentsExpCourse = await _context.StudentCourse_ExpCourses
                        .Where(x => x.Exp_CourseId == expCourse.Id).ToListAsync();

                    float sum = 0;
                    studentsExpCourse.ForEach(x =>
                    {
                        sum += x.FinalGrade;
                    });

                    float avg = sum / studentsExpCourse.Count;

                    Object row1 = new { 
                        Experiment_Name = exp.Name, 
                        No_Students = studentsExpCourse.Count, 
                        Avarage_Degree = avg
                    };
        

                    data.Add(row1);
                }

                IEnumerable<object> dataTable = data;
                //Assign data source.
                pdfGrid.DataSource = dataTable;
                //Draw grid to the page of PDF document.
                pdfGridLayoutResult = pdfGrid.Draw(page, new Syncfusion.Drawing.PointF(0, 60));
                //pdfGrid.Draw(page, new Syncfusion.Drawing.PointF(0, pdfGridLayoutResult.Bounds.Bottom + 20));
            }

            Stream stream = new MemoryStream();

            document.Save(stream);

            stream.Position = 0;

            var url = createFile(stream, teacher.FirstName+"-Courses");

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
           Policy = "ViewAnalytics_Teacher")]
        [HttpGet("Course")]
        public async Task<IActionResult> getCoourse(int page, string searchByCourseName, string searchByCourseCode)
        {
            string host = HttpContext.Request.Host.Value;

            searchByCourseName += "";
            searchByCourseCode += "";

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

            var courses = _context.User_Courses.Where(x=>x.UserId == teacher.Id)
                .Select(x=>x.Course).Where(x=>x.Name.ToLower().Contains(""+ searchByCourseName.ToLower())
                && x.Code.ToLower().Contains("" + searchByCourseCode.ToLower())).ToList();

            var count = courses.Count;
            var coursesDto = courses.Skip(page * 10).Take(10).Select(x => _mapper.Map<CourseDto>(x)).ToList();

            return Ok(new Result()
            {
                Status = true,
                Data = new
                {
                    result = coursesDto,
                    count,
                    next = (page * 10) + 10 >= count ? null : $"https://{host}/api/Analytics/Course?page={page + 1 + 1}",
                    previous = page == 0 ? null : $"https://{host}/api/Analytics/Course?page={page - 1 + 1}"
                }
            });
        }

        [HttpGet("Course/Report")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
           Policy = "ViewAnalytics_Teacher")]
        public async Task<IActionResult> getCourseReport(string courseIdd)
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

            var courses = _context.User_Courses.Where(x => x.UserId == teacher.Id)
                .Select(x => x.Course).ToList();

            PdfDocument document = new PdfDocument();

            foreach (var course in courses)
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
                graphics.DrawString($"Course ID: {course.Idd}", font, PdfBrushes.Black, new Syncfusion.Drawing.PointF(0, 0));
                graphics.DrawString($"Course Code: {course.Code}", font, PdfBrushes.Black, new Syncfusion.Drawing.PointF(0, 20));
                graphics.DrawString($"Course Name: {course.Name}", font, PdfBrushes.Black, new Syncfusion.Drawing.PointF(0, 40));
                graphics.DrawString($"Number of Experiments in course: {course.NumberOfExp}", font, PdfBrushes.Black, new Syncfusion.Drawing.PointF(0, 60));

                var expCourses = await _context.Exp_Courses.Where(x => x.CourseId == course.Id).ToListAsync();

                PdfGrid pdfGrid = new PdfGrid();

                PdfGridLayoutResult pdfGridLayoutResult;
                List<object> data = new List<object>();

                foreach (var expCourse in expCourses)
                {
                    var exp = await _context.Expirments.FindAsync(expCourse.ExperimentId);
                    var studentsExpCourse = await _context.StudentCourse_ExpCourses
                        .Where(x => x.Exp_CourseId == expCourse.Id).ToListAsync();

                    float sum = 0;
                    studentsExpCourse.ForEach(x =>
                    {
                        sum += x.FinalGrade;
                    });

                    float avg = sum / studentsExpCourse.Count;

                    Object row1 = new
                    {
                        Experiment_ID = exp.Idd,
                        Experiment_Name = exp.Name,
                        No_Students = studentsExpCourse.Count,
                        Avarage_Degree = avg
                    };


                    data.Add(row1);
                }

                IEnumerable<object> dataTable = data;
                //Assign data source.
                pdfGrid.DataSource = dataTable;
                //Draw grid to the page of PDF document.
                pdfGridLayoutResult = pdfGrid.Draw(page, new Syncfusion.Drawing.PointF(0, 110));
                //pdfGrid.Draw(page, new Syncfusion.Drawing.PointF(0, pdfGridLayoutResult.Bounds.Bottom + 20));
            }

            Stream stream = new MemoryStream();

            document.Save(stream);

            stream.Position = 0;

            var url = createFile(stream, teacher.FirstName + "-Courses");

            return Ok(new { url });
        }

        private static Stream GenerateStreamFromString(string s)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }
        private static void CopyStream(Stream input, Stream output)
        {
            byte[] buffer = new byte[8 * 1024];
            int len;
            while ((len = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                output.Write(buffer, 0, len);
            }
        }
    }
}
