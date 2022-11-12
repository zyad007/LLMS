using LLS.DAL.Interfaces;
using LLS.DAL.Repositories;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLS.DAL.Data
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly AppDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;


        public UnitOfWork(AppDbContext context,
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public IUserRepository Users => new UserRepository(_context);
        public ITokenRepository Tokens => new TokenRepository(_context);
        public ICourseRepository Courses => new CourseRepository(_context);
        public IExperimentRepository Experiments => new ExperimentRepository(_context);
        public IRoleRepository Roles => new RoleRepository(_userManager, _roleManager, _context);


        public void Dispose()
        {
            _context.Dispose();
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}