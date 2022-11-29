using AutoMapper;
using LLS.BLL.IServices;
using LLS.Common.Dto;
using LLS.Common.Models;
using LLS.Common.Models.LLO;
using LLS.Common.Transfere_Layer_Object;
using LLS.DAL.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LLS.BLL.Services
{

    public class ExperimentService : IExperimentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _iMapper;
        public ExperimentService(IUnitOfWork unitOfWork,
            IMapper iMapper)
        {
            _unitOfWork = unitOfWork;
            _iMapper = iMapper;
        }

        public async Task<Result> CreateExp(ExpDto expDto)
        {
            //var exist = await _unitOfWork.Experiments.GetByIdd(expDto.Idd);
            //if(exist != null)
            //{
            //    return new Result()
            //    {
            //        Status = false,
            //        Message = "There is Expirment with the same IDD"
            //    };
            //}

            //var exp = _iMapper.Map<Experiment>(expDto);

            //FetchDtoIntoLLO(exp, expDto);

            var exp = new Experiment()
            {
                Name = expDto.Name,
                Description = expDto.Description,
                Idd = expDto.Idd,
                //AuthorName = To do 
            };

            var result = await _unitOfWork.Experiments.Create(exp);
            await _unitOfWork.SaveAsync();

            if (result == false)
            {
                return new Result()
                {
                    Status = false,
                    Message = "Something went wrong"
                };
            }

            return new Result()
            {
                Status = true,
                Message = "Added Successfully",
                Data = exp.Idd
            };
        }

        public async Task<Result> DeleteExp(Guid idd)
        {
            var exp = await _unitOfWork.Experiments.GetByIdd(idd);
            if (exp == null)
            {
                return new Result()
                {
                    Status = false,
                    Message = "There is no Experiment with this IDD"
                };
            }

            _unitOfWork.Experiments.Delete(exp);
            await _unitOfWork.SaveAsync();

            return new Result()
            {
                Status = true,
                Message = "Deleted Successfully"
            };
        }

        public async Task<Result> GetAllExp()
        {
            var exps = await _unitOfWork.Experiments.GetAll();
            var expsDto = new List<ExpDto>();

            foreach (var exp in exps)
            {
                var expDto = _iMapper.Map<ExpDto>(exp);
                //if (exp.LLO != null)
                //{
                //    expDto.LLO = JsonConvert.DeserializeObject<LLO>(exp.LLO);
                //}

                expsDto.Add(expDto);
            }

            return new Result()
            {
                Status = true,
                Data = expsDto
            };
        }

        public async Task<Result> GetExpByIdd(Guid idd)
        {
            var exp = await _unitOfWork.Experiments.GetByIdd(idd);
            if (exp == null)
            {
                return new Result()
                {
                    Status = false,
                    Message = "There is no Experiment with this IDD"
                };
            }

            var expDto = _iMapper.Map<ExpDto>(exp);
            //if(exp.LLO != null)
            //{
            //    expDto.LLO = JsonConvert.DeserializeObject<LLO>(exp.LLO);
            //}

            //if (exp.LLO_MA != null)
            //{
            //    expDto.LLO_MA = JsonConvert.DeserializeObject<LLO>(exp.LLO_MA);
            //}
            

            return new Result()
            {
                Status = true,
                Data = expDto
            };
        }

        public async Task<Result> UpdateExp(ExpDto expDto)
        {
            var exist = await _unitOfWork.Experiments.GetByIdd(expDto.Idd);
            if (exist == null)
            {
                return new Result()
                {
                    Status = false,
                    Message = "There is no Experiment with this IDD"
                };
            }

            var exp = _iMapper.Map<Experiment>(expDto);

            FetchDtoIntoLLO(exp, expDto.LLO);

            await _unitOfWork.Experiments.Update(exp);
            await _unitOfWork.SaveAsync();

            return new Result()
            {
                Status = true,
                Message = "Updated Successfully",
                Data = expDto
            };
        }

        public async Task<Result> CreateLLO(Guid expIdd,LLO llo)
        {
            var exp = await _unitOfWork.Experiments.GetByIdd(expIdd);
            if (exp == null)
            {
                return new Result()
                {
                    Status = false,
                    Message = "There is no Experiment with that IDD"
                };
            }

            FetchDtoIntoLLO(exp, llo);

            await _unitOfWork.Experiments.Update(exp);
            await _unitOfWork.SaveAsync();

            return new Result()
            {
                Status = true,
                Message = "LLO added Successfully"
            };

        }

        public async Task<Result> GetLLO(Guid expIdd)
        {
            var exp = await _unitOfWork.Experiments.GetByIdd(expIdd);
            if (exp == null)
            {
                return new Result()
                {
                    Status = false,
                    Message = "There is no Experiment with that IDD"
                };
            }

            if(exp.LLO != null)
            {
                var llo = JsonConvert.DeserializeObject<LLO>(exp.LLO);

                return new Result()
                {
                    Status = true,
                    Data = llo
                };
            }

            return new Result()
            {
                Status = false,
                Message = "No llo for this exp yet"
            };
        }

        private void FetchDtoIntoLLO(Experiment exp, LLO llo)
        {
            //exp.LLO_MA = JsonConvert.SerializeObject(expDto.LLO, Formatting.None,
            //                    new JsonSerializerSettings
            //                    {
            //                        NullValueHandling = NullValueHandling.Ignore
            //                        //ContractResolver = new IgnorePropertiesResolver(new[]{"prop1"})
            //                    });

            //for (int i = 0; i < expDto.LLO.Pages.Count; i++)
            //{
            //    for (int j = 0; j < expDto.LLO.Pages[i].Blocks.Count; j++)
            //    {
            //        expDto.LLO.Pages[i].Blocks[j].Score = null;
            //    }
            //}

            exp.LLO = JsonConvert.SerializeObject(llo, Formatting.None,
                                        new JsonSerializerSettings
                                        {
                                            NullValueHandling = NullValueHandling.Ignore
                                        });
        }


        public class IgnorePropertiesResolver : DefaultContractResolver
        {
            private readonly HashSet<string> ignoreProps;
            public IgnorePropertiesResolver(IEnumerable<string> propNamesToIgnore)
            {
                this.ignoreProps = new HashSet<string>(propNamesToIgnore);
            }

            protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
            {
                JsonProperty property = base.CreateProperty(member, memberSerialization);
                if (this.ignoreProps.Contains(property.PropertyName))
                {
                    property.ShouldSerialize = _ => false;
                }
                return property;
            }
        }
    }
}
