using LLS.Common.Dto.Logins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static LLS.BLL.Services.AccountService;

namespace LLS.BLL.IServices
{
    public interface IAccountService
    {
        Task<AuthResult> Login(Login login);
        Task<AuthResult> RefreshToken(TokenRequest tokenRequest);
    }
}
