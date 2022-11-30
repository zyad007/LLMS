using LLS.BLL.IServices;
using LLS.Common.Dto.Logins;
using LLS.Common.Transfere_Layer_Object;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using static LLS.BLL.Services.AccountService;

namespace LLS.API.Controllers
{
    public class AccountController : BaseController
    {
        private readonly IAccountService _accountService;
        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(Login login)
        {
            if(ModelState.IsValid)
            {
                var authRes = await _accountService.Login(login);
                return CheckResult(AuthToRes(authRes));
            }

            return BadRequest(new Result()
            {
                Message = "Invaild Payload",
                Status = false
            }
            );
        }

        [HttpPost("RefreshToken")]
        public async Task<IActionResult> RefreshToken(TokenRequest tokenRequest)
        {
            if(ModelState.IsValid)
            {
                var authRes = await _accountService.RefreshToken(tokenRequest);
                return CheckResult(AuthToRes(authRes));

            }

            return BadRequest(new Result()
            {
                Message = "Invaild Payload",
                Status = false
            }
            );
        }

        private Result AuthToRes(AuthResult authResult)
        {
            return new Result()
            {
                Data = new
                {
                    authResult.Token,
                    authResult.RefreshToken,
                    authResult.Email,
                    authResult.Role,
                    authResult.Idd
                },
                Message = authResult.Error,
                Status = authResult.Status
            };
        }
    }
}
