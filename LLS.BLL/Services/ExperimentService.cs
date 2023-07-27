using AutoMapper;
using LLS.BLL.IServices;
using LLS.Common.Dto;
using LLS.Common.Models;
using LLS.Common.Models.LLO;
using LLS.Common.Transfere_Layer_Object;
using LLS.DAL.Data;
using LLS.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;
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
        private readonly AppDbContext _context;
        public ExperimentService(IUnitOfWork unitOfWork,
            IMapper iMapper,
            AppDbContext context)
        {
            _unitOfWork = unitOfWork;
            _iMapper = iMapper;
            _context = context;
        }

        public async Task<Result> CreateExp(ExpDto expDto, string userId)
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
                AuthorId = userId,
                AuthorName = expDto.AuthorName,
                hasLLO = false,
                Editable = true
            };

            var result = await _unitOfWork.Experiments.Create(exp);
            await _unitOfWork.SaveAsync();
            
            var expDb = await _context.Expirments.FirstOrDefaultAsync(x=>x.Idd == exp.Idd);

            if (expDb == null)
            {
                return new Result()
                {
                    Status = false,
                    Message = "Something went wrong"
                };
            }

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
                Data = _iMapper.Map<ExpDto>(expDb)
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

        public async Task<Result> GetAllExp(int page, string searchByName, string searchByRelatedCourse, string host)
        {

            if (page == -1)
            {
                page = 0;
            }

            //var exps = await _unitOfWork.Experiments.GetAllExps();

            var exps = _context.Expirments.Where(x=>x.Active == true).ToList();

            //var searchExp = exps.Where(x => x.Name.Contains("" + searchByName)).ToList();
            var searchExp = exps.Where(x=>((x.Name + "").ToLower() + "").Contains(("" + searchByName).ToLower())).ToList();

            var count = searchExp.Count;

            var pagedExp = searchExp.Skip(page * 10).Take(10).ToList();

            var expsDto = new List<ExpDto>();

            foreach (var exp in pagedExp)
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
                Data = new
                {
                    result = expsDto,
                    count,
                    next = (page*10)+10 >= count ? null : $"https://{host}/api/Experiment?page={page+1+1}",
                    previous = page == 0 ? null : $"https://{host}/api/Experiment?page={page-1+1}"
                }
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

            if (!exp.Active)
            {
                var exp_Courses = _context.Exp_Courses.FirstOrDefault(x=>x.ExperimentId == exp.Id);
                if(exp_Courses != null)
                {
                    var course = _context.Courses.FirstOrDefault(x => x.Id == exp_Courses.CourseId);
                    expDto.CourseIdd = course.Idd;
                    expDto.CourseName = course.Name;
                }
            }

            return new Result()
            {
                Status = true,
                Data = expDto
            };
        }

        public async Task<Result> UpdateExp(ExpDto expDto, Guid userId)
        {
            var exp = await _unitOfWork.Experiments.GetByIdd(expDto.Idd);
            if (exp == null)
            {
                return new Result()
                {
                    Status = false,
                    Message = "There is no Experiment with this IDD"
                };
            }

            if(exp.AuthorId != userId.ToString())
            {
                var expCopy = exp;
                expCopy.Name = expDto.Name;
                expCopy.Description = expDto.Description;
                expCopy.Id = Guid.NewGuid();
                expCopy.Idd = Guid.NewGuid();
                expCopy.AddedDate = DateTime.Now;
                expCopy.UpdateDate = DateTime.Now;
                expCopy.Active = false;

                await _unitOfWork.Experiments.Create(expCopy);

                var exp_ress = await _context.Resource_Exps.Where(x => x.ExperimentId == exp.Id).ToListAsync();
                foreach (var exp_resTemp in exp_ress)
                {
                    var exp_res = new Resource_Exp()
                    {
                        ExperimentId = expCopy.Id,
                        Experiment = expCopy,
                        Resource = exp_resTemp.Resource,
                        ResourceId = exp_resTemp.ResourceId
                    };

                    await _context.Resource_Exps.AddAsync(exp_res);
                }

                var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
                expCopy.AuthorName = user.Email;

                await _unitOfWork.SaveAsync();

                return new Result()
                {
                    Data = _iMapper.Map<ExpDto>(expCopy),
                    Status = true,
                    Message = "A new Copy Added Successfully"
                };
            }
            else
            {
                var ex = _iMapper.Map<Experiment>(expDto);

                await _unitOfWork.Experiments.Update(ex);
                await _unitOfWork.SaveAsync();

                return new Result()
                {
                    Data = _iMapper.Map<ExpDto>(ex),
                    Status = true,
                    Message = "Updated Successfully"
                };
            }

        }

        public async Task<Result> CreateLLO(Guid expIdd,LLO llo, string userId)
        {
            var exp = await _context.Expirments.FirstOrDefaultAsync(x=>x.Idd == expIdd);
            if (exp == null)
            {
                return new Result()
                {
                    Status = false,
                    Message = "There is no Experiment with that IDD"
                };
            }

            if(exp.AuthorId != userId)
            {
                return new Result()
                {
                    Status = false,
                    Message = "The user is not the Original User"
                };
            }

            if(exp.Editable == false)
            {
                return new Result()
                {
                    Status = false,
                    Message = "This Experiment is not editable because it's in live progress for student"
                };
            }

            //FetchDtoIntoLLO(exp, llo);

            exp.LLO = JsonConvert.SerializeObject(llo, Formatting.None,
                                        new JsonSerializerSettings
                                        {
                                            NullValueHandling = NullValueHandling.Ignore
                                        });

            exp.hasLLO = true;

            await _context.SaveChangesAsync();

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

        public async Task<Result> AddRecources(List<Guid> resIdds, Guid idd)
        {
            var exp = await _unitOfWork.Experiments.GetByIdd(idd);
            if (exp == null)
            {
                return new Result()
                {
                    Status = false,
                    Message = "There is no Experiment with that IDD"
                };
            }

            foreach(var resIdd in resIdds)
            {
                var resDb = _context.Resources.FirstOrDefault(x=> x.Id == resIdd);
                if(resDb == null)
                {
                    return new Result()
                    {
                        Status = false,
                        Message = "no recource with this Id"
                    };
                }

                var exist = _context.Resource_Exps.FirstOrDefault(x => x.ResourceId == resDb.Id
                                                                    && x.ExperimentId == exp.Id);
                if(exist != null)
                {
                    return new Result()
                    {
                        Status = false,
                        Message = "recource already in this experiment"
                    };
                }

                var res_exp = new Resource_Exp()
                {
                    ExperimentId = exp.Id,
                    Experiment = exp,
                    ResourceId = resDb.Id,
                    Resource = resDb
                };

                await _context.Resource_Exps.AddAsync(res_exp);
            }

            await _unitOfWork.SaveAsync();

            return new Result()
            {
                Status = true,
                Message = "resources added succesfully"
            };
        }

        public async Task<Result> GetResource(Guid idd)
        {
            var exp = await _unitOfWork.Experiments.GetByIdd(idd);
            if (exp == null)
            {
                return new Result()
                {
                    Status = false,
                    Message = "There is no Experiment with that IDD"
                };
            }

            var resourcesDtos = await _context.Resource_Exps.Where(x => x.ExperimentId == exp.Id)
                                .Select(x => new ResourceDto()
                                {
                                    Id = x.Resource.Id,
                                    Name = x.Resource.Name
                                }).ToListAsync();

            return new Result()
            {
                Status = true,
                Data = resourcesDtos
            };
        }

        public async Task<Result> RemoveRecource(Guid idd, Guid resIdd)
        {
            var exp = await _unitOfWork.Experiments.GetByIdd(idd);
            if (exp == null)
            {
                return new Result()
                {
                    Status = false,
                    Message = "There is no Experiment with that IDD"
                };
            }

            var resDb = _context.Resources.FirstOrDefault(x => x.Id == resIdd);
            if (resDb == null)
            {
                return new Result()
                {
                    Status = false,
                    Message = "no recource with this Id"
                };
            }

            var res_exp = _context.Resource_Exps.FirstOrDefault(x => x.ResourceId == resDb.Id
                                                                    && x.ExperimentId == exp.Idd);

            if(res_exp == null)
            {
                return new Result()
                {
                    Status = false,
                    Message = "recource is not in this experiment"
                };
            }

            _context.Resource_Exps.Remove(res_exp);

            await _unitOfWork.SaveAsync();

            return new Result()
            {
                Status = true,
                Message = "recource removed successfully"
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
