using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLS.Common.Models
{
    public class StudentSession
    {
        public Guid Id { get; set; }

        public Guid StudentId { get; set; }
        public User Student { get; set; }

        public Guid ExpCourseId { get; set; }
        public Exp_Course ExpCourse { get; set; }

        public Guid? MachineId { get; set; }
        public Machine? Machine { get; set; }


        // Time Slot index
        public int TimeSlot { get; set; }
    }
}
