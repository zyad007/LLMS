using LLS.BLL.IServices;
using LLS.Common.Dto;
using LLS.Common.Dto.Logins;
using LLS.Common.Transfere_Layer_Object;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LLS.API.Controllers
{
    public class UserController : BaseController
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("SignUp")]
        public async Task<IActionResult> SingUp(SignUp signUp)
        {
            if(ModelState.IsValid)
            {
                var res = await _userService.AddUser(signUp);
                return CheckResult(res);
            }

            return BadRequest(new Result
            {
                Status = false,
                Message = "Invalid Payload"
            });
        }

        //To Do remove email and make it with JWT only
        [HttpGet("email")]
        public async Task<IActionResult> GetUserByEmail(string email)
        {
            var res = await _userService.GetUserByEmail(email);

            return CheckResult(res);
        }

        [HttpGet("{idd}")]
        public async Task<IActionResult> GetUserByIdd(Guid userIdd)
        {
            var res = await _userService.GetUserByIdd(userIdd);

            return CheckResult(res);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUser()
        {
            var res = await _userService.GetAllUsers();

            return CheckResult(res);
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> DeleteUser(Guid userIdd)
        {
            var res = await _userService.DeleteUser(userIdd);

            return CheckResult(res);
        }

        [HttpPut("Update")]
        public async Task<IActionResult> UpdateUser(UserDto userDto)
        {
            var res = await _userService.UpdateUser(userDto);

            return CheckResult(res);
        }

        [HttpGet("Get-All-Users-With-Role")]
        public async Task<IActionResult> GetAllUsersWithRole(string role)
        {
            var res = await _userService.GetAllUserWithRole(role);

            return CheckResult(res);
        }
    }
}
