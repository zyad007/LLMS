using LLS.Common.Transfere_Layer_Object;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLS.DAL.Interfaces
{
    public interface IRoleRepository
    {
        Task<Result> AddRole(string roleName);
        Task<Result> GetAllUsers();
        Task<Result> AddUserToRole(Guid idd, string email);
        Task<Result> GetUserRoles(string email);
        Task<Result> RemoveRoleFromUser(Guid idd, string roleName);
        Task<Result> DeleteRole(Guid idd);
    }
}
