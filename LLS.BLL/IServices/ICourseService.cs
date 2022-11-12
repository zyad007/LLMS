using LLS.Common.Dto;
using LLS.Common.Transfere_Layer_Object;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLS.BLL.IServices
{
    public interface ICourseService
    {
        Task<Result> CreateCourse(CourseDto courseDto);
        Task<Result> DeleteCourse(string idd);
        Task<Result> GetAllCourses();
        Task<Result> GetCourse(string idd);
        Task<Result> UpdateCourse(CourseDto courseDto);

        Task<Result> AssignUserToCourse(string email, string idd, string role);
        Task<Result> GetUsersAssignedToCourse(string idd, string role);


        Task<Result> AssignExpToCourse(Guid expIdd, string courseIdd, DateTime startDate, DateTime endDate, int trials, List<Guid> resourcesId);
        Task<Result> GetExpAssignedToCourse(string idd);


    }
}
