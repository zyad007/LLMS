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
        public string AuthorId { get; set; }
        public string AuthorName { get; set; }
        public LLO LLO { get; set; }
        public LLO LLO_MA { get; set; }
        public string Description { get; set; }
        public string CourseName { get; set; }
        public string CourseIdd { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<Resource> Resources { get; set; }
    }
}
