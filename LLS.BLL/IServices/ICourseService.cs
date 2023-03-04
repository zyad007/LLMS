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
        Task<Result> DeleteCourse(Guid idd);
        Task<Result> GetAllCourses(int page);
        Task<Result> GetCourse(Guid idd);
        Task<Result> UpdateCourse(CourseDto courseDto);

        Task<Result> AssignUserToCourse(Guid userIdd, Guid idd, string role);
        Task<Result> GetUsersAssignedToCourse(Guid idd, string role, int page);


        Task<Result> AssignExpToCourse(Guid expIdd, Guid courseIdd, DateTime startDate, DateTime endDate, int trials);
        Task<Result> GetExpAssignedToCourse(Guid idd, int page);


    }
}
