using AutoMapper;
using LLS.BLL.IServices;
using LLS.Common.Dto;
using LLS.Common.Models;
using LLS.Common.Models.LLO;
using LLS.Common.Transfere_Layer_Object;
using LLS.DAL.Data;
using LLS.DAL.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace LLS.API.Controllers
{
    public class ExperimentController : BaseController
    {
        private readonly IExperimentService _experimentService;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly AppDbContext _context;
        private readonly IMapper _iMapper;
        public ExperimentController(IExperimentService experimentService
                                    , UserManager<IdentityUser> userManager,
                                    AppDbContext context,
                                    IMapper mapper)
        {
            _experimentService = experimentService;
            _userManager = userManager;
            _context = context;
            _iMapper = mapper;
        }


        [HttpGet]
        public async Task<IActionResult> GetAllExp([FromQuery] int page,[FromQuery] string searchByName, string searchByRelatedCourse)
        {
            var res = await _experimentService.GetAllExp(page - 1, searchByName, searchByRelatedCourse, HttpContext.Request.Host.Value);
            return CheckResult(res);
        }

        [HttpGet("{idd}")]
        public async Task<IActionResult> GetExpByIdd(Guid idd)
        {
            var res = await _experimentService.GetExpByIdd(idd);
            return CheckResult(res);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Policy = "AddDeleteEdit_Exp")]
        [HttpPost]
        public async Task<IActionResult> CreateExp([FromBody] CreateExp create)
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var userEmail = HttpContext.User.FindFirst(ClaimTypes.Email).Value;

            var expDto = new ExpDto()
            {
                Name = create.name,
                Description = create.description,
                AuthorName = userEmail,
                Idd = Guid.NewGuid()
            };
            var res = await _experimentService.CreateExp(expDto, userId);
            return CheckResult(res);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Policy = "AddDeleteEdit_Exp")]
        [HttpPost("{idd}/LLO")]
        public async Task<IActionResult> CreateLLO(Guid idd, LLO llo)
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var res = await _experimentService.CreateLLO(idd, llo, userId);
            return CheckResult(res);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Policy = "AddDeleteEdit_Exp")]
        [HttpPut("{idd}/LLO")]
        public async Task<IActionResult> EditLLO(Guid idd, LLO llo)
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var res = await _experimentService.CreateLLO(idd, llo, userId);

            return CheckResult(res);
        }

        [HttpGet("{idd}/LLO")]
        public async Task<IActionResult> GetLLOByIdd(Guid idd)
        {
            var res = await _experimentService.GetLLO(idd);
            return CheckResult(res);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Policy = "AddDeleteEdit_Exp")]
        [HttpDelete("{idd}")]
        public async Task<IActionResult> DeleteExp(Guid idd)
        {
            var res = await _experimentService.DeleteExp(idd);
            return CheckResult(res);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Policy = "AddDeleteEdit_Exp")]
        [HttpPut("{idd}")]
        public async Task<IActionResult> UpdateExp(Guid idd,[FromBody]CreateExp expUpdate)
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var expDto = new ExpDto()
            {
                Idd = idd,
                Name = expUpdate.name,
                Description = expUpdate.description
            };

            var res = await _experimentService.UpdateExp(expDto, new Guid(userId));
            return CheckResult(res);
        }


        [HttpGet("{idd}/Export")]
        public async Task<FileStreamResult> export(Guid idd)
        {
            var exp =await _context.Expirments.FirstOrDefaultAsync(x=>x.Idd==idd);

            if(exp ==null)
            {
                throw new BadHttpRequestException("No exp with tihs Idd");
            }

            var expJson = JsonConvert.SerializeObject(new
            {
                exp.Name,
                exp.Description,
                exp.LLO
            });
            
            var stream = GenerateStreamFromString(expJson);
            
            return new FileStreamResult(stream, new MediaTypeHeaderValue("text/plain"))
            {
                FileDownloadName = $"{exp.Name}.txt"
            };
            
        }

        private class ExpJson
        {
            public string Name { get; set; }
            public string Description { get; set; }
            public string LLO { get; set; }
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Policy = "AddDeleteEdit_Exp")]
        [HttpPost("Import")]
        public async Task<IActionResult> import(IFormFile lloFile)
        {
            var expJson = ReadLLO(lloFile);

            try
            {
                var expObj = JsonConvert.DeserializeObject<ExpJson>(expJson);
                var LLO = JsonConvert.DeserializeObject<LLO>(expObj.LLO);

                var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                var userEmail = HttpContext.User.FindFirst(ClaimTypes.Email).Value;


                var expDto = new ExpDto()
                {
                    Name = expObj.Name,
                    Description = expObj.Description,
                    AuthorName = userEmail,
                    Idd = Guid.NewGuid()
                };

                var exp = new Experiment()
                {
                    Name = expDto.Name,
                    Description = expDto.Description,
                    Idd = expDto.Idd,
                    AuthorId = userId,
                    AuthorName = expDto.AuthorName,
                    LLO = expObj.LLO,
                    hasLLO = true
                };

                var result = await _context.Expirments.AddAsync(exp);
                await _context.SaveChangesAsync();


                return Ok(new Result()
                {
                    Status = true,
                    Message = "Added Successfully",
                    Data = _iMapper.Map<ExpDto>(result.Entity)
                });

            }
            catch(Exception ex)
            {
                return BadRequest(new Result()
                {
                    Status = false,
                    Message = "File is corrupted - "+ex.Message
                });
            }


            

        }

        public static string ReadLLO(IFormFile file)
        {
            var result = new StringBuilder();
            using (var reader = new StreamReader(file.OpenReadStream()))
            {
               return reader.ReadLine();
            }
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

        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
        //    Policy = "AddDeleteEdit_Exp")]
        //[HttpPost("{idd}/Add-Resources")]
        //public async Task<IActionResult> AddResources(Guid idd,[FromBody] List<Guid> resIdds)
        //{
        //    var result = await _experimentService.AddRecources(resIdds, idd);
        //    return CheckResult(result);
        //}

        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
        //    Policy = "AddDeleteEdit_Exp")]
        //[HttpDelete("{idd}/Remove-Resource")]
        //public async Task<IActionResult> RemoveResources(Guid idd, Guid resIdd)
        //{
        //    var result = await _experimentService.RemoveRecource(idd, resIdd);
        //    return CheckResult(result);
        //}

        //[HttpGet("{idd}/Get-Resource")]
        //public async Task<IActionResult> GetResources(Guid idd)
        //{
        //    var result = await _experimentService.GetResource(idd);
        //    return CheckResult(result);
        //}
    }

    public class CreateExp
    {
        public string name { get; set; }
        public string description { get; set; }
    }
}
