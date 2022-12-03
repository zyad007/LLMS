using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLS.Common.Dto.Logins
{
    public class AuthResult
    {
        public string Email { get; set; }
        public string Role { get; set; }
        public Guid Idd { get; set; }
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public bool Status { get; set; }
        public object Error { get; set; }
        public List<string> Permissions { get; set; }
    }
}
