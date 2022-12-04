using LLS.Common.Models;
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
    public class ExperimentRepository : Repostiroy<Experiment>, IExperimentRepository
    {
        public ExperimentRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<Experiment> GetByIdd(Guid idd)
        {
            return await _set.FirstOrDefaultAsync(x => x.Idd == idd);
        }

        public async Task<List<Experiment>> GetAllExps()
        {
            return await _set.Where(x => x.Active == true).ToListAsync();
        }

        public async Task Update(Experiment exp)
        {
            var res = await _set.FirstOrDefaultAsync(x=>x.Idd == exp.Idd);

            res.Description = exp.Description;
            res.Name = exp.Name;

            res.UpdateDate = DateTime.Now;
        }
    }
}
