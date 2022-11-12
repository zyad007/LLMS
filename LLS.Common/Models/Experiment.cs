using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLS.Common.Models
{
    public class Experiment : BaseEntity
    {
        public string Name { get; set; }
        public Guid Idd { get; set; } = Guid.NewGuid();
        public string AuthorId { get; set; }
        public string AuthorName { get; set; }
        public string LLO { get; set; }     //LLO Default no socre
        public string LLO_MA {get; set;}    //LLO Model Answer
        public string LLO_SA { get; set; }  //LLO Student Answer
        public string Description { get; set; } 

        //Assigned Courses for this Expirment
        public List<Exp_Course> Exp_Courses { get; set; }

    }
}
