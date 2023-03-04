using AutoMapper;
using LLS.BLL.IServices;
using LLS.Common.Dto;
using LLS.Common.Models;
using LLS.Common.Models.LLO;
using LLS.Common.Transfere_Layer_Object;
using LLS.DAL.Data;
using LLS.DAL.Interfaces;
using LLS.DAL.Migrations;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLS.BLL.Services
{
    public class TeacherService : ITeacherService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        public TeacherService(IUnitOfWork unitOfWork, AppDbContext context, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _context = context;
            _mapper = mapper;
        }

        public async Task<Result> GetCourses(Guid userIdd, int page, bool isTeacher)
        {
            if (page == -1)
            {
                page = 0;
            }

            var user = _context.Users.FirstOrDefault(x=>x.IdentityId == userIdd);

            if (user == null)
            {
                return new Result()
                {
                    Message = "User doesn't exist",
                    Status = false
                };
            }

            //if (user.Role.ToLower() != "teacher")
            //{
            //    return new Result()
            //    {
            //        Message = "User doesn't have Teacher Role",
            //        Status = false
            //    };
            //}

            var courses = _context.User_Courses.Where(x => x.UserId == user.Id && x.Role == "teacher").Select(x=>x.Course).ToList();

            var count = courses.Count;

            var pagedCourse = courses.Skip(page * 10).Take(10).ToList();

            var coursesDto = new List<CourseDto>();
            foreach (var course in pagedCourse)
            {
                var courseDto = _mapper.Map<CourseDto>(course);
                coursesDto.Add(courseDto);
            }

            string path;

            if(isTeacher)
            {
                path = "​/api​/Teacher​/Courses";
            }else
            {
                path = "/api/Course";
            }

            return new Result()
            {
                Data = new
                {
                    result = coursesDto,
                    count,
                    next = (page * 10) + 10 >= count ? null : $"https://stupefied-antonelli.74-50-88-98.plesk.page{path}?page={page + 1 + 1}",
                    previous = page == 0 ? null : $"https://stupefied-antonelli.74-50-88-98.plesk.page{path}?page={page - 1 + 1}"
                },
                Status = true
            };
        }

        public async Task<Result> getExps(Guid teacherId, Guid courseIdd, int page)
        {
            if (page == -1)
            {
                page = 0;
            }

            var user = _context.Users.FirstOrDefault(x => x.IdentityId == teacherId);
            if (user == null)
            {
                return new Result()
                {
                    Message = "User doesn't exist",
                    Status = false
                };
            }

            //if (user.Role.ToLower() != "teacher")
            //{
            //    return new Result()
            //    {
            //        Message = "User doesn't have Teacher Role",
            //        Status = false
            //    };
            //}

            var course = _context.Courses.FirstOrDefault(x=> x.Idd == courseIdd);

            if (course == null)
            {
                return new Result()
                {
                    Message = "There is no course with this idd",
                    Status = false
                };
            }

            var exps = _context.Exp_Courses.Where(x => x.CourseId == courseIdd).Select(x => x.Experiment).ToList();
            var count = exps.Count;

            var expsPaged = exps.Skip(page * 10).Take(10).ToList();


            var expDtos = new List<ExpDto>();
            foreach (var exp in expsPaged)
            {
                var expDto = _mapper.Map<ExpDto>(exp);
                expDtos.Add(expDto);
            }

            return new Result()
            {
                Data = new
                {
                    result = expDtos,
                    count,
                    next = (page * 10) + 10 >= count ? null : $"https://stupefied-antonelli.74-50-88-98.plesk.page/api/Teacher/Courses/{courseIdd}/Experiments?page={page + 1 + 1}",
                    previous = page == 0 ? null : $"https://stupefied-antonelli.74-50-88-98.plesk.page/api/Teacher/Courses/{courseIdd}/Experiments?page={page - 1 + 1}"
                },
                Status = true
            };
        }

        public async Task<Result> GetGradeBookForExp(Guid userId, Guid courseIdd, Guid expIdd, int page)
        {
            if (page == -1)
            {
                page = 0;
            }

            var user = _context.Users.FirstOrDefault(x => x.IdentityId == userId);
            if (user == null)
            {
                return new Result()
                {
                    Message = "User doesn't exist",
                    Status = false
                };
            }

            //if (user.Role.ToLower() != "teacher")
            //{
            //    return new Result()
            //    {
            //        Message = "User doesn't have Teacher Role",
            //        Status = false
            //    };
            //}

            var course = _context.Courses.FirstOrDefault(x=>x.Idd == courseIdd);

            if (course == null)
            {
                return new Result()
                {
                    Message = "There is no course with this idd",
                    Status = false
                };
            }

            //if(_context.User_Courses.Where(x=> x.CourseId == course.Id && x.UserId == user.Id).FirstOrDefault() ==  null)
            //{
            //    return new Result()
            //    {
            //        Message = "Teacher is not assgined to this course",
            //        Status = false
            //    };
            //}

            var exp = await _context.Expirments.FirstOrDefaultAsync(x=> x.Idd == expIdd);
            if(exp == null)
            {
                return new Result()
                {
                    Message = "exp doesn't exist course",
                    Status = false
                };
            }

            var expCourse = _context.Exp_Courses.FirstOrDefault(x=>x.CourseId == course.Id && x.ExperimentId == exp.Id);

            if(expCourse == null)
            {
                return new Result()
                {
                    Message = "exp is not assigned to the course",
                    Status = false
                };
            }


            var student_expsCourse = _context.StudentCourse_ExpCourses.Where(x => x.Exp_CourseId == expCourse.Id).ToList();
            var count = student_expsCourse.Count;

            var student_expsCoursePaged = student_expsCourse.Skip(page * 10).Take(10).ToList();

            return new Result()
            {
                Data = new
                {
                    result = student_expsCourse.Select(x => new
                    {
                        x.Id,
                        x.NumberOfTials,
                        _context.Users.FirstOrDefault(z => z.Id == _context.User_Courses.FirstOrDefault(y => y.Id == x.Student_CourseId).UserId).Email
                    }).ToList(),
                    count,
                    next = (page * 10) + 10 >= count ? null : $"https://stupefied-antonelli.74-50-88-98.plesk.page/api/Teacher/Courses/{courseIdd}/Experiments/{expIdd}/Grade-Books?page={page + 1 + 1}",
                    previous = page == 0 ? null : $"https://stupefied-antonelli.74-50-88-98.plesk.page/api/Teacher/Courses/{courseIdd}/Experiments/{expIdd}/Grade-Books?page={page - 1 + 1}"
                },
                Status = true
            };
        }

        public async Task<Result> getStudentTrials(Guid teacherId, Guid studentCourseExpId, Guid expIdd, Guid courseIdd, int page)
        {
            if (page == -1)
            {
                page = 0;
            }

            var user = _context.Users.FirstOrDefault(x => x.IdentityId == teacherId);
            if (user == null)
            {
                return new Result()
                {
                    Message = "User doesn't exist",
                    Status = false
                };
            }

            //if (user.Role.ToLower() != "teacher")
            //{
            //    return new Result()
            //    {
            //        Message = "User doesn't have Teacher Role",
            //        Status = false
            //    };
            //}

            var trials = _context.Trials.Where(x=>x.StudentCourse_ExpCourseId== studentCourseExpId).ToList();

            if (!trials.Any())
            {
                return new Result()
                {
                    Message = "There is no trials yet completed by student",
                    Status = false
                };
            }


            int count = trials.Count;

            var trialsPaged = trials.Skip(page * 10).Take(10).ToList();

            

            return new Result()
            {
                Data = new
                {
                    results= trialsPaged.Select(x => new
                    {
                        x.Id,
                        x.IsGraded,
                        x.TotalTimeInMin,
                        x.StartedAt,
                        x.SubmitedAt,
                        x.Status
                    }).ToList(),
                    count,
                    next = (page * 10) + 10 >= count ? null : $"https://stupefied-antonelli.74-50-88-98.plesk.page/api/Teacher/Courses/{courseIdd}/Experiments/{expIdd}/Grade-Books/{studentCourseExpId}/Trials?page={page + 1 + 1}",
                    previous = page == 0 ? null : $"https://stupefied-antonelli.74-50-88-98.plesk.page/api/Teacher/Courses/{courseIdd}/Experiments/{expIdd}/Grade-Books/{studentCourseExpId}/Trials?page={page - 1 + 1}"
                },
                Status = true
            };
        }

        public async Task<Result> getStudentLRO( Guid teacherId,Guid expIdd, Guid trialId)
        {
            var user = _context.Users.FirstOrDefault(x => x.IdentityId == teacherId);
            if (user == null)
            {
                return new Result()
                {
                    Message = "User doesn't exist",
                    Status = false
                };
            }

            //if (user.Role.ToLower() != "teacher")
            //{
            //    return new Result()
            //    {
            //        Message = "User doesn't have Teacher Role",
            //        Status = false
            //    };
            //}

            var exp = await _context.Expirments.FirstOrDefaultAsync(x=>x.Idd ==expIdd);
            if (exp == null)
            {
                return new Result()
                {
                    Message = "exp doesn't exist",
                    Status = false
                };
            }

            if(!exp.hasLLO)
            {
                return new Result()
                {
                    Message = "exp doesn't has LLO",
                    Status = false
                };
            }

            var trial = await _context.Trials.FirstOrDefaultAsync(x=> x.Id== trialId);

            if(trial == null )
            {
                return new Result()
                {
                    Message = "trial not found",
                    Status = false
                };
            }



            if(!String.IsNullOrEmpty(trial.LLA) && !String.IsNullOrEmpty(exp.LLO) )
            {
                AutoGrade(); //Generates LRO from LLA
                LLO LRO;  // To be changed
                if(trial.IsGraded)
                {
                    LRO = JsonConvert.DeserializeObject<LLO>(trial.LRO);
                }else
                {
                    LRO = JsonConvert.DeserializeObject<LLO>(trial.LLA);
                }

                var LLO = JsonConvert.DeserializeObject<LLO>(exp.LLO);

                return new Result()
                {
                    Data = new
                    {
                        LRO,
                        LLO
                    },
                    Status = true
                };
            }

            return new Result()
            {
                Message = "LLO or LRO is null",
                Status = true
            };

        }

        private void AutoGrade()
        {

        }

        public async Task<Result> GradeStudentTrial(Guid teacherId , Guid trialId, LLO lro)
        {
            var user = _context.Users.FirstOrDefault(x => x.IdentityId == teacherId);
            if (user == null)
            {
                return new Result()
                {
                    Message = "User doesn't exist",
                    Status = false
                };
            }

            //if (user.Role.ToLower() != "teacher")
            //{
            //    return new Result()
            //    {
            //        Message = "User doesn't have Teacher Role",
            //        Status = false
            //    };
            //}

            var trial = await _context.Trials.FirstOrDefaultAsync(x => x.Id == trialId);

            if (trial == null)
            {
                return new Result()
                {
                    Message = "trial not found",
                    Status = false
                };
            }

            trial.Status = "graded";
            trial.IsGraded = true;

            trial.LRO = JsonConvert.SerializeObject(lro);

            await _unitOfWork.SaveAsync();

            return new Result()
            {
                Message = "Graded Successfully",
                Status = true
            };
        }
    }
}
