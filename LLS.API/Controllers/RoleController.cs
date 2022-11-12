using LLS.DAL.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace LLS.API.Controllers
{
    public class RoleController : BaseController
    {
        private readonly IUnitOfWork _unitOfWork;
        public RoleController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpPost("add-role")]
        public async Task<IActionResult> AddRole(string roleName)
        {
            var result = await _unitOfWork.Roles.AddRole(roleName);
            return CheckResult(result);
        }

        [HttpGet("get-all-roles")]
        public async Task<IActionResult> GetAllRoles()
        {
            var result = await _unitOfWork.Roles.GetAllUsers();
            return CheckResult(result);
        }

        [HttpPost("add-user-to-role")]
        public async Task<IActionResult> AddUserToRole(string roleName, string email)
        {
            var result = await _unitOfWork.Roles.AddUserToRole(roleName, email);
            await _unitOfWork.SaveAsync();
            return CheckResult(result);
        }

        [HttpGet("get-user-roles")]
        public async Task<IActionResult> GetUserRoles(string email)
        {
            var result = await _unitOfWork.Roles.GetUserRoles(email);
            return CheckResult(result);
        }

        [HttpPost("remove-role-from-user")]
        public async Task<IActionResult> RemoveRoleFromUser(string email, string roleName)
        {
            var result = await _unitOfWork.Roles.RemoveRoleFromUser(email, roleName);
            await _unitOfWork.SaveAsync();
            return CheckResult(result);
        }

        [HttpDelete("delete-role")]
        public async Task<IActionResult> DeleteRole(string roleName)
        {
            var result = await _unitOfWork.Roles.DeleteRole(roleName);
            await _unitOfWork.SaveAsync();
            return CheckResult(result);
        }
    }
}
