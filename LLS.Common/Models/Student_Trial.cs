using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLS.Common.Models
{
    public class Student_Trial
    {
        public Guid Id { get; set; }

        public Guid StudentCourse_ExpCourseId { get; set; }

        [ForeignKey(nameof(StudentCourse_ExpCourseId))]
        public StudentCourse_ExpCourse StudentCourse_ExpCourse { get; set; }

        public int TrialNumber {get;set;}

        public bool IsGraded {get;set;}
        public float TotalScore { get; set; }
        public float TotalTimeInMin { get; set; }

        public string LLA { get; set; }
        public string LRO { get; set; }

        public DateTime SubmitedAt { get; set; }
        public DateTime StartedAt { get; set; }

        public string Status { get; set; }
        
    }
}
