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
    public class CourseRepository : Repostiroy<Course>, ICourseRepository
    {
        public CourseRepository(AppDbContext context) : base(context)
        {

        }

        public async Task<Course> GetByIdd(string idd)
        {
            return await _set.FirstOrDefaultAsync(x => x.Idd.ToLower() == idd.ToLower());
        }

        public async Task Update(Course course)
        {
            var res = await _set.FirstOrDefaultAsync(x => x.Idd == course.Idd);

            res.Name = course.Name;
            res.StartDate = course.StartDate;
            res.EndDate = course.EndDate;
            res.Description = course.Description;

            res.UpdateDate = DateTime.Now;
        }
    }
}
