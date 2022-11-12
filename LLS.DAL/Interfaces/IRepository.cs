using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLS.DAL.Interfaces
{
    public interface IRepository<T> where T : class
    {
        Task<bool> Create(T entity);
        void Delete(T entity);
        Task<List<T>> GetAll();
        Task<T> GetById(Guid Id);
    }
}
