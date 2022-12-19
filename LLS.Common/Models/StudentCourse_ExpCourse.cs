using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLS.Common.Models
{
    public class StudentCourse_ExpCourse
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        //Relation
        public Guid Student_CourseId { get; set; } 

        [ForeignKey(nameof(Student_CourseId))]
        public User_Course Student_Course { get; set; }


        public Guid Exp_CourseId { get; set; }
        
        [ForeignKey(nameof(Exp_CourseId))]
        public Exp_Course Exp_Course { get; set; }

        
        public List<Student_Trial> Trials { get; set; }
        public int NumberOfTials { get; set; } = 0;
        public bool IsCompleted { get; set; }

    }
}
