using LLS.Common.Models.LLO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLS.Common.Dto
{
    public class TrialDto
    {
        public int TrialNumber { get; set; }

        public bool IsGraded = true;
        public float TotalScore { get; set; }
        public float TotalTimeInMin { get; set; }

        public LLO LRO_SA { get; set; }
        public LLO LRO { get; set; }
    }
}
