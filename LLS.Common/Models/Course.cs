﻿using LLS.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLS.Common.Models
{
    public class Course : BaseEntity
    {
        
        public string Name { get; set; }
        public string Idd { get; set; }
        public string Description { get; set; }
        public string CreatedBy { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public int NumberOfStudents { get; set; }
        public int NumberOfExp { get; set; }


        //Assigend Student To Course
        public List<User_Course> User_Courses { get; set; }

        //Assigned Exp to Course
        public List<Exp_Course> Exp_Courses { get; set; }

    }
}