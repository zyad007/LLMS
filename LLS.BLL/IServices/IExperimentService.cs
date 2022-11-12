using LLS.Common.Dto;
using LLS.Common.Models.LLO;
using LLS.Common.Transfere_Layer_Object;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLS.BLL.IServices
{
    public interface IExperimentService
    {
        Task<Result> CreateExp(ExpDto expDto);
        Task<Result> CreateLLO(Guid expIdd, LLO llo);
        Task<Result> GetLLO(Guid expIdd);
        Task<Result> UpdateExp(ExpDto expDto);
        Task<Result> DeleteExp(Guid idd);
        Task<Result> GetExpByIdd(Guid idd);
        Task<Result> GetAllExp();

    }
}
