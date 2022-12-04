using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLS.Common.Dto
{
    public class UserDto
    {
        public Guid Idd { get; set; }
        public string FirstName { get; set; }
        public string Lastname { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Country { get; set; }
        public string Role { get; set; }
        public string AcademicYear { get; set; }
        public string City { get; set; }
        public string Gender { get; set; }
    }
}
