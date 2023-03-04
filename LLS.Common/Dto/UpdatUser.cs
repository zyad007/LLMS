using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLS.Common.Dto
{
    public class UpdateUser
    {
        public string email { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string phoneNumber { get; set; }
        public string country { get; set; }
        public string academicYear { get; set; }
        public string City { get; set; }
        public string Gender { get; set; }
        public string Role { get; set; }
    }
}
