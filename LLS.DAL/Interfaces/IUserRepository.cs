using LLS.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLS.DAL.Interfaces
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User> GetByEmail(string email);
        Task Update(User user);
    }
}
