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
        Task<Result> CreateExp(ExpDto expDto, string userId);
        Task<Result> CreateLLO(Guid expIdd, LLO llo, string userId);
        Task<Result> GetLLO(Guid expIdd);
        Task<Result> UpdateExp(ExpDto expDto, Guid userId);
        Task<Result> DeleteExp(Guid idd);
        Task<Result> GetExpByIdd(Guid idd);
        Task<Result> GetAllExp(int page);
        Task<Result> AddRecources(List<Guid> resIdds, Guid idd);
        Task<Result> RemoveRecource(Guid idd, Guid resIdd);
        Task<Result> GetResource(Guid idd);
    }
}
