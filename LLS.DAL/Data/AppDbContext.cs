using LLS.Common.Models;
using LLS.Common.Models.LLO;
using LLS.Common.Models.LLO.Pages;
using LLS.Common.Models.LLO.Pages.Blocks;
using LLS.Common.Models.LLO.Pages.Blocks.Content;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLS.DAL.Data
{
    public class AppDbContext : IdentityDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            //Student_Course
            builder.Entity<User_Course>()
                .HasOne(b => b.User)
                .WithMany(ba => ba.User_Courses)
                .HasForeignKey(bi => bi.UserId);

            builder.Entity<User_Course>()
                .HasOne(b => b.Course)
                .WithMany(ba => ba.User_Courses)
                .HasForeignKey(bi => bi.CourseId);

            //Exp_Course
            builder.Entity<Exp_Course>()
                .HasOne(b => b.Experiment)
                .WithMany(ba => ba.Exp_Courses)
                .HasForeignKey(bi => bi.ExperimentId);

            builder.Entity<Exp_Course>()
                .HasOne(b => b.Course)
                .WithMany(ba => ba.Exp_Courses)
                .HasForeignKey(bi => bi.CourseId);

            //StudentCourse_ExpCourse
            builder.Entity<StudentCourse_ExpCourse>()
                .HasOne(b => b.Student_Course)
                .WithMany(ba => ba.StudentCourse_ExpCourses)
                .HasForeignKey(bi => bi.Student_CourseId);

            builder.Entity<StudentCourse_ExpCourse>()
                .HasOne(b => b.Exp_Course)
                .WithMany(ba => ba.StudentCourse_ExpCourses)
                .HasForeignKey(bi => bi.Exp_CourseId);


            builder.Entity<Student_Trial>()
                .HasOne(b => b.StudentCourse_ExpCourse)
                .WithMany(x => x.Trials)
                .HasForeignKey(x => x.Student_ExpCourseId);

            // Resource_Machine
            builder.Entity<Resource_Machine>()
                .HasOne(b => b.Resource)
                .WithMany(ba => ba.resource_machines)
                .HasForeignKey(bi => bi.ResourceId);

            builder.Entity<Resource_Machine>()
                .HasOne(b => b.Machine)
                .WithMany(ba => ba.resource_machines)
                .HasForeignKey(bi => bi.MachineId);



            base.OnModelCreating(builder);
        }

        public DbSet<User> Users { get; set; }

        public DbSet<RefreshToken> RefreshTokens { get; set; }

        public DbSet<Experiment> Expirments { get; set; }

        public DbSet<Course> Courses { get; set; }

        //public DbSet<Machine> Machines { get; set; }

        //public DbSet<Resource> Resources { get; set; }

        public DbSet<StudentSession> StudentSessions { get; set; }

        //Joint Db
        public DbSet<Exp_Course> Exp_Courses { get; set; }
        public DbSet<User_Course> User_Courses { get; set; }
        public DbSet<StudentCourse_ExpCourse> StudentCourse_ExpCourses { get; set; }
        //public DbSet<Resource_Machine> Resource_Machines { get; set; }
        public DbSet<Student_Trial> Trials { get; set; }
        //public DbSet<Resource_Exp> Resource_Exps { get; set; }


    }
}
