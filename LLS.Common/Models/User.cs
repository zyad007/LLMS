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
        public Guid Idd { get; set; } = Guid.NewGuid();
        public string FirstName { get; set; }
        public string Lastname { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Country { get; set; }
        public string Role { get; set; } = "USER";
        public string AcademicYear { get; set; }
        public string City { get; set; }
        public string Gender { get; set; }
        public string imgURL { get; set; }
        public DateTime LastLogIn { get; set; }


        //Assigned Courses for Student
        public List<User_Course> User_Courses { get; set; }
    }
}
