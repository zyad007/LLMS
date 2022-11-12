using LLS.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLS.DAL.Interfaces
{
    public interface ICourseRepository : IRepository<Course>
    {
        Task<Course> GetByIdd(string idd);
        Task Update(Course course);
    }
}
