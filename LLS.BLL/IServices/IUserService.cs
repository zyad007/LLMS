using LLS.Common.Dto;
using LLS.Common.Dto.Logins;
using LLS.Common.Models;
using LLS.Common.Transfere_Layer_Object;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLS.BLL.IServices
{
    public interface IUserService
    {
        Task<Result> AddUser(SignUp signUp);
        Task<Result> UpdateUser(UserDto userDto);
        Task<Result> DeleteUser(Guid userIdd);
        Task<Result> GetUserByEmail(string email);
        Task<Result> GetUserByIdd(Guid userIdd);
        Task<Result> GetAllUsers();
        Task<Result> GetAllUserWithRole(string role);
    }
}
