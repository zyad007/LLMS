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
    public class UserRepository : Repostiroy<User>, IUserRepository
    {
        public UserRepository(AppDbContext context):base(context)
        {

        }

        public async Task<User> GetByEmail(string email)
        {
            return await _set.FirstOrDefaultAsync(x=> x.Email.ToLower() == email.ToLower());
        }

        public async Task<User> GetByIdd(Guid idd)
        {
            return await _set.FirstOrDefaultAsync(x => x.Idd == idd);
        }


        public async Task Update(User user)
        {
            var res = await _set.FirstOrDefaultAsync(x => x.Idd == user.Idd);

            res.FirstName = user.FirstName;
            res.Lastname = user.Lastname;
            res.PhoneNumber = user.PhoneNumber;
            res.Country = user.Country;
            res.AcademicYear = user.AcademicYear;
            res.City = user.City;
            res.Gender = user.Gender;
            res.Role = user.Role;
            res.imgURL = user.imgURL;

            res.UpdateDate = DateTime.Now;
        }
    }
}
