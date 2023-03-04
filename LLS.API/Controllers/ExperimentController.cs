using LLS.BLL.IServices;
using LLS.Common.Dto;
using LLS.Common.Models;
using LLS.Common.Models.LLO;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;

namespace LLS.API.Controllers
{
    public class ExperimentController : BaseController
    {
        private readonly IExperimentService _experimentService;
        private readonly UserManager<IdentityUser> _userManager;
        public ExperimentController(IExperimentService experimentService
                                    , UserManager<IdentityUser> userManager)
        {
            _experimentService = experimentService;
            _userManager = userManager;
        }


        [HttpGet]
        public async Task<IActionResult> GetAllExp([FromQuery] int page)
        {
            var res = await _experimentService.GetAllExp(page - 1);
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
