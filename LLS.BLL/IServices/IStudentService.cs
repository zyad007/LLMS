using LLS.Common.Dto;
using LLS.Common.Transfere_Layer_Object;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static LLS.BLL.Services.StudentService;

namespace LLS.BLL.IServices
{
    public interface IStudentService
    {
        Task<Result> GetAssignedExpForStudent(string email, int page, string searchByName, string searchByCourseName);
        Task<Result> GetStudentCourses(string email, int page, string searchByName, string searchByCode);
        Task<Result> GetCompletedExp(string email, int page,string searchByName, string searchByCourseName);
        //Task<Result> GetStudentResult(string email, string courseIdd, Guid expIdd);
        //Task<Result> ReserveTimeSlot(string email, Guid expIdd, string courseIdd, int timeSlot);


        //New
        Task<Result> startTrial(Guid expIdd, string studentId);
        Task<Result> SubmitTrial(TrialSubmit submitTrial, string studentId);
    }
}
