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
        public LLO lla { get; set; }
        public Guid expIdd { get; set; }
        public Guid courseIdd { get; set; }
        public float TotalTimeInMin { get; set; }
    }
}
