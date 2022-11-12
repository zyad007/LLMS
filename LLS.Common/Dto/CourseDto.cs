using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLS.Common.Dto
{
    public class CourseDto
    {
        public string Name { get; set; }
        public string Idd { get; set; }
        public string Description { get; set; }
        public string CreatedBy { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public int NumberOfStudents { get; set; }
        public int NumberOfExp { get; set; }
    }
}
