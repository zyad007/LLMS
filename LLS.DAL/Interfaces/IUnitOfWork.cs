using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLS.DAL.Interfaces
{
    public interface IUnitOfWork
    {
        IUserRepository Users { get; }
        ITokenRepository Tokens { get; }
        ICourseRepository Courses { get; }
        IExperimentRepository Experiments { get; }
        IRoleRepository Roles { get; }
        Task SaveAsync();
    }
}
