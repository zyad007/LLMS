using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLS.Common.Models
{
    public class Exp_Course
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid ExperimentId { get; set; }
        public Guid CourseId { get; set; }


        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int NumbersOfTrials { get; set; }


        //Relation
        [ForeignKey(nameof(ExperimentId))]
        public Experiment Experiment { get; set; }

        [ForeignKey(nameof(CourseId))]
        public Course Course { get; set; }


        //Info for Specifiec Student in This Exp
        public List<StudentCourse_ExpCourse> StudentCourse_ExpCourses { get; set; }
    }
}
