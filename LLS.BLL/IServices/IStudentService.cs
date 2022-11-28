using LLS.Common.Dto;
using LLS.Common.Transfere_Layer_Object;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLS.BLL.IServices
{
    public interface IStudentService
    {
        Task<Result> GetAssignedExpForStudent(string email);
        Task<Result> GetStudentCourses(string email);
        //Task<Result> SubmitExp(StudentSubmit submit);
        //Task<Result> GetStudentResult(string email, string courseIdd, Guid expIdd);
        //Task<Result> ReserveTimeSlot(string email, Guid expIdd, string courseIdd, int timeSlot);
    }
}
