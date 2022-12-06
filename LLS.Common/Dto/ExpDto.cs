using LLS.Common.Models;
using LLS.Common.Models.LLO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLS.Common.Dto
{
    public class ExpDto
    {
        public Guid Idd { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
        public string AuthorName { get; set; }
        public LLO LLO { get; set; }
        public string Description { get; set; }
        public string CourseName { get; set; }
        public Guid CourseIdd { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime UpdateDate { get; set; }
    }
}
