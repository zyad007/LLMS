using LLS.Common.Models.LLO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLS.Common.Dto
{
    public class StudentSubmit
    {
        public LLO LRO_SA { get; set; }
        public Guid expIdd { get; set; }
        public string email { get; set; }
        public string courseIdd { get; set; }
        public float TotalTimeInMin { get; set; }
    }
}
