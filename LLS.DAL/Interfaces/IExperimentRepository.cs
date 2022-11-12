using LLS.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLS.DAL.Interfaces
{
    public interface IExperimentRepository : IRepository<Experiment>
    {
        Task<Experiment> GetByIdd(Guid idd);
        Task Update(Experiment exp);
    }
}
