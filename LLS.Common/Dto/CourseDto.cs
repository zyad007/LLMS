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
        public Guid Idd { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public int NumberOfStudents { get; set; }
        public int NumberOfExp { get; set; }
    }
}
