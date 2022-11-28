using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLS.Common.Models
{
    public class User : BaseEntity
    {
        public Guid IdentityId { get; set; }
        public string FirstName { get; set; }
        public string Lastname { get; set; }
        public string Email { get; set; }
        public int PhoneNumber { get; set; }
        public string Country { get; set; }
        public string Role { get; set; } = "User";
        public string AcademicYear { get; set; }

        //Assigned Courses for Student
        public List<User_Course> User_Courses { get; set; }
    }
}
