﻿using LLS.BLL.IServices;
using LLS.Common.Dto;
using LLS.Common.Models;
using LLS.Common.Models.LLO;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace LLS.API.Controllers
{
    public class ExperimentController : BaseController
    {
        private readonly IExperimentService _experimentService;
        public ExperimentController(IExperimentService experimentService)
        {
            _experimentService = experimentService;
        }


        [HttpGet]
        public async Task<IActionResult> GetAllExp()
        {
            var res = await _experimentService.GetAllExp();
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
        [HttpPost("Create")]
        public async Task<IActionResult> CreateExp([FromBody]CreateExp create) 
        {
            var expDto = new ExpDto()
            {
                Name = create.name,
                Description = create.description,
                Idd = Guid.NewGuid()
            };
            var res = await _experimentService.CreateExp(expDto);
            return CheckResult(res);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Policy = "AddDeleteEdit_Exp")]
        [HttpPost("CreateLLO")]
        public async Task<IActionResult> CreateLLO(Guid idd, LLO llo)
        {
            var res = await _experimentService.CreateLLO(idd, llo);
            return CheckResult(res);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Policy = "AddDeleteEdit_Exp")]
        [HttpPut("{idd}/EditLLO")]
        public async Task<IActionResult> EditLLO(Guid idd, LLO llo)
        {
            var res = await _experimentService.CreateLLO(idd, llo);
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
        [HttpDelete("Delete")]
        public async Task<IActionResult> DeleteExp(Guid idd)
        {
            var res = await _experimentService.DeleteExp(idd);
            return CheckResult(res);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Policy = "AddDeleteEdit_Exp")]
        [HttpPut("/{idd}/Update")]
        public async Task<IActionResult> UpdateExp(Guid idd,[FromBody]CreateExp expUpdate)
        {
            var expDto = new ExpDto()
            {
                Idd = idd,
                Name = expUpdate.name,
                Description = expUpdate.description
            };
            var res = await _experimentService.UpdateExp(expDto);
            return CheckResult(res);
        }



    }

    public class CreateExp
    {
        public string name { get; set; }
        public string description { get; set; }
    }
}
