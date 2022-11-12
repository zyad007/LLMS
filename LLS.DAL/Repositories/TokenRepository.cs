using LLS.Common.Models;
using LLS.DAL.Data;
using LLS.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLS.DAL.Repositories
{
    public class TokenRepository : Repostiroy<RefreshToken>, ITokenRepository
    {
        public TokenRepository(AppDbContext context) : base(context)
        {

        }
    }
}
