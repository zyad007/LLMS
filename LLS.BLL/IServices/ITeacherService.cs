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
        Task<Result> GetTeacherCourses(string email);
    }
}
