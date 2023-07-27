using LLS.Common.Dto;
using LLS.Common.Dto.Logins;
using LLS.Common.Models;
using LLS.Common.Transfere_Layer_Object;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static LLS.BLL.Services.UserService;

namespace LLS.BLL.IServices
{
    public interface IUserService
    {
        Task<Result> AddUser(RegisterUser user);
        Task<Result> RegisterUserWithConfirmToken(RegisterUser user, string role);
        Task<AuthResult> ConfirmAccount(string email, string password, string token);
        Task<Result> UpdateUser(UserDto userDto);
        Task<Result> DeleteUser(Guid userIdd);
        Task<Result> GetUserByEmail(string email);
        Task<Result> GetUserByIdd(Guid userIdd);
        Task<Result> GetAllUsers(Guid courseIdd, int page, string role, string searchByEmail, string searchByFirstName, string searchByLastName); // To Filter All Already assigned users form the list
                                                  // if courseIdd is provided in Quyers.
        Task<Result> GetAllUserWithRole(string role);
    }
}
