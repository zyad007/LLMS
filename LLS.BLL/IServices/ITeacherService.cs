using LLS.Common.Models.LLO;
using LLS.Common.Transfere_Layer_Object;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLS.BLL.IServices
{
    public interface ITeacherService
    {
        Task<Result> getExps(Guid teacherId, Guid courseIdd, int page);
        Task<Result> GetGradeBookForExp(Guid userId, Guid courseIdd, Guid expIdd, int page);
        Task<Result> GetCourses(Guid userIdd, int page, bool isTeacher);
        Task<Result> getStudentTrials(Guid teacherId, Guid studentCourseExpId,Guid expIdd, Guid courseIdd, int page);
        Task<Result> getStudentLRO(Guid teacherId, Guid expIdd, Guid trialId);
        Task<Result> GradeStudentTrial(Guid teacherId, Guid trialId, LLO lro);

    }
}
