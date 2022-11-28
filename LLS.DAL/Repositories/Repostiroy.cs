using LLS.DAL.Data;
using LLS.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLS.DAL.Repositories
{
    public class Repostiroy<T> : IRepository<T> where T : class
    {
        private readonly AppDbContext _context;
        protected DbSet<T> _set;
        public Repostiroy(AppDbContext context)
        {
            _context = context;
            _set = context.Set<T>();
        }

        // Check if entity exist retrun boolean
        // Generic Update Method using the _context.Update();

        public async Task<bool> Create(T entity)
        { 
            var res = await _set.AddAsync(entity);
            if (res != null)
                return true;

            return false;
        }

        public void Delete(T entity)
        {
            _set.Remove(entity);
        }

        public async Task<List<T>> GetAll()
        {
            return await _set.ToListAsync();
        }

        public async Task<T> GetById(Guid id)
        {
            return await _set.FindAsync(id);
        }
    }
}
