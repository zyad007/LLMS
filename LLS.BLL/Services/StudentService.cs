using AutoMapper;
using LLS.BLL.Algorithms;
using LLS.BLL.IServices;
using LLS.Common.Dto;
using LLS.Common.Models;
using LLS.Common.Models.LLO;
using LLS.Common.Models.LLO.Pages.Blocks;
using LLS.Common.Transfere_Layer_Object;
using LLS.DAL.Data;
using LLS.DAL.Interfaces;
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
    public class StudentService : IStudentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        public StudentService(IUnitOfWork unitOfWork, AppDbContext context, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _context = context;
            _mapper = mapper;
        }

        //private async Task<Result> GetAssignedExpForStudentv2(string email, int page, string searchByName, string searchByCourseName)
        //{
        //    if (page == -1)
        //    {
        //        page = 0;
        //    }

        //    var user = await _unitOfWork.Users.GetByEmail(email);
        //    if(user == null)
        //    {
        //        return new Result()
        //        {
        //            Message = "User doesn't exist",
        //            Status = false
        //        };
        //    }

        //    if(user.Role.ToLower() != "student")
        //    {
        //        return new Result()
        //        {
        //            Message = "User doesn't have Student Role",
        //            Status = false
        //        };
        //    }

        //    var user_Courses = await _context.User_Courses.Where(n => n.UserId == user.Id)
        //                      .ToListAsync();

        //    var Exp_Courses = new List<Exp_Course>();

        //    foreach (var user_course in user_Courses)
        //    {
        //        var temp = await _context.StudentCourse_ExpCourses
        //            .Where(x => !x.IsCompleted && x.Student_CourseId == user_course.Id)
        //            .Select(x => x.Exp_Course).ToListAsync();
        //        Exp_Courses.AddRange(temp);
        //    }

        //    try
        //    {
        //        if (!Exp_Courses.Any())
        //        {
        //            return new Result()
        //            {
        //                Message = "There is no assigned experiment",
        //                Status = false
        //            };
        //        }
        //    }
        //    catch (ArgumentNullException)
        //    {
        //        return new Result()
        //        {
        //            Message = "There is no experiments assigned for user ex",
        //            Status = false
        //        };
        //    }

        //    var Exp_CooursesPaged = Exp_Courses.Skip(page * 10).Take(10).ToList();
        //    var count = Exp_Courses.Count;

        //    var expsDto = new List<ExpDto>();
        //    foreach (var exp_course in Exp_CooursesPaged)
        //    {
        //        var exp = await _context.Expirments.FirstOrDefaultAsync(x => x.Id == exp_course.ExperimentId);
        //        var course = await _context.Courses.FirstOrDefaultAsync(x => x.Id == exp_course.CourseId);

        //        var expDto = new ExpDto()
        //        {
        //            Idd = exp.Idd,
        //            Name = exp.Name,
        //            Description = exp.Description,
        //            AuthorName = exp.AuthorName,
        //            CourseName = course.Name,
        //            CourseIdd = course.Idd,
        //            StartDate = exp_course.StartDate,
        //            EndDate = exp_course.EndDate
        //        };

        //        expsDto.Add(expDto);
        //    }


        //    return new Result()
        //    {
        //        Data = new
        //        {
        //            result = expsDto.Where(x=>x.Name.Contains(""+searchByName) || x.CourseName.Contains(""+searchByCourseName)).ToList(),
        //            count,
        //            next = (page * 10) + 10 >= count ? null : $"https://stupefied-antonelli.74-50-88-98.plesk.page/api/Student/Assigned-Experiments?page={page + 1 + 1}",
        //            previous = page == 0 ? null : $"https://stupefied-antonelli.74-50-88-98.plesk.page/api/Student/Assigned-Experiments?page={page - 1 + 1}"
        //        },
        //        Status = true
        //    };

        //}

        public async Task<Result> GetAssignedExpForStudent(string email, int page, string searchByName, string searchByCourseName)
        {
            searchByName += "";
            searchByCourseName += "";

            if (page == -1)
            {
                page = 0;
            }

            var user = await _unitOfWork.Users.GetByEmail(email);

            if (user == null)
            {
                return new Result()
                {
                    Message = "User doesn't exist",
                    Status = false
                };
            }

            if (user.Role.ToLower() != "student")
            {
                return new Result()
                {
                    Message = "User doesn't have Student Role",
                    Status = false
                };
            }

            var expDto = await _context.StudentCourse_ExpCourses
                .Where( x => x.Student_Course.UserId == user.Id && !x.IsCompleted )
                .Select(x=> new ExpDto()
                {
                    Idd = x.Exp_Course.Experiment.Idd,
                    Name = x.Exp_Course.Experiment.Name,
                    CourseName = x.Exp_Course.Course.Name,
                    ReservedAt = (x.StartFrom.ToString()==("0001-01-01 00:00:00")) ? null : x.StartFrom,
                    Status = x.Status,
                    //isAvailable = (x.StartFrom <= DateTime.Now.AddHours(2) && x.EndAt >= DateTime.Now.AddHours(2))
                    isAvailable = x.Status == "Reserved"
                })
                .Where(x => x.Name.ToLower().Contains("" + searchByName.ToLower()) && x.CourseName.ToLower().Contains("" + searchByCourseName.ToLower()))
                .ToListAsync();

            var sortedList = new List<object>();

            var i1 = new List<int>();
            for (int i = 0; i < expDto.Count; i++)
            {
                if (expDto[i].isAvailable == true)
                {
                    sortedList.Add(expDto[i]);
                    i1.Add(i);
                }
            }
            for (int i = i1.Count - 1; i >= 0; i--)
            {
                expDto.RemoveAt(i1[i]);
            }

            var i2 = new List<int>();
            for (int i = 0; i < expDto.Count; i++)
            {
                if (expDto[i].Status == "Assinged")
                {
                    sortedList.Add(expDto[i]);
                    i2.Add(i);
                }
            }
            for (int i = i2.Count - 1; i >= 0; i--)
            {
                expDto.RemoveAt(i2[i]);
            }

            var i3 = new List<int>();
            for (int i = 0; i < expDto.Count; i++)
            {
                if (expDto[i].Status == "Reserved")
                {
                    sortedList.Add(expDto[i]);
                    i3.Add(i);
                }
            }
            for (int i = i3.Count - 1; i >= 0; i--)
            {
                expDto.RemoveAt(i3[i]);
            }

            sortedList.AddRange(expDto);

            var count = expDto.Count;

            return new Result()
            {
                Data = new
                {
                    result = sortedList.Skip(page * 10).Take(10).ToList(),
                    count,
                    next = (page * 10) + 10 >= count ? null : $"https://optimistic-burnell.74-50-88-98.plesk.page/api/Student/Assigned?page={page + 1 + 1}",
                    previous = page == 0 ? null : $"https://optimistic-burnell.74-50-88-98.plesk.page/api/Student/Assigned?page={page - 1 + 1}"
                },
                Status = true
            };
        }

        //private async Task<Result> GetCompletedExpv2(string email, int page,string searchByName,string searchByCourseName)
        //{
        //    if (page == -1)
        //    {
        //        page = 0;
        //    }

        //    var user = await _unitOfWork.Users.GetByEmail(email);
        //    if (user == null)
        //    {
        //        return new Result()
        //        {
        //            Message = "User doesn't exist",
        //            Status = false
        //        };
        //    }

        //    if (user.Role.ToLower() != "student")
        //    {
        //        return new Result()
        //        {
        //            Message = "User doesn't have Student Role",
        //            Status = false
        //        };
        //    }

        //    var user_Courses = await _context.User_Courses.Where(n => n.UserId == user.Id)
        //                      .ToListAsync();

        //    var Exp_Courses = new List<Exp_Course>();

        //    foreach (var user_course in user_Courses)
        //    {
        //        var temp = await _context.StudentCourse_ExpCourses.Where(x => x.IsCompleted && x.Student_CourseId == user_course.Id)
        //                        .Select(x => x.Exp_Course).ToListAsync();
        //        Exp_Courses.AddRange(temp);
        //    }

        //    try { if (!Exp_Courses.Any())
        //            {
        //                return new Result()
        //                {
        //                    Message = "There is no completed experiment",
        //                    Status = false
        //                };
        //            }
        //    }
        //    catch (ArgumentNullException)
        //    {
        //        return new Result()
        //        {
        //            Message = "There is no completed experiment ex",
        //            Status = false
        //        };
        //    }

        //    var Exp_CoursesPaged = Exp_Courses.Skip(page * 10).Take(10).ToList();
        //    var count = Exp_Courses.Count;

        //    var expsDto = new List<ExpDto>();
        //    foreach (var exp_course in Exp_CoursesPaged)
        //    {
        //        var exp = await _context.Expirments.FirstOrDefaultAsync(x => x.Id == exp_course.ExperimentId);
        //        var course = await _context.Courses.FirstOrDefaultAsync(x => x.Id == exp_course.CourseId);
        //        var expDto = new ExpDto()
        //        {
        //            Name = exp.Name,
        //            Description = exp.Description,
        //            AuthorName = exp.AuthorName,
        //            CourseName = course.Name,
        //            CourseIdd = course.Idd,
        //            StartDate = exp_course.StartDate,
        //            EndDate = exp_course.EndDate
        //        };

        //        expsDto.Add(expDto);
        //    }


        //    return new Result()
        //    {
        //        Data = new
        //        {
        //            result = expsDto.Where(x => x.Name.Contains("" + searchByName) || x.CourseName.Contains("" + searchByCourseName)).ToList(),
        //            count,
        //            next = (page * 10) + 10 >= count ? null : $"https://stupefied-antonelli.74-50-88-98.plesk.page/api/Student/Completed-Experiments?page={page + 1 + 1}",
        //            previous = page == 0 ? null : $"https://stupefied-antonelli.74-50-88-98.plesk.page/api/Student/Completed-Experiments?page={page - 1 + 1}"
        //        },
        //        Status = true
        //    };

        //}

        public async Task<Result> GetCompletedExp(string email, int page, string searchByName, string searchByCourseName)
        {
            searchByName += "";
            searchByCourseName += "";

            if (page == -1)
            {
                page = 0;
            }

            var user = await _unitOfWork.Users.GetByEmail(email);

            if (user == null)
            {
                return new Result()
                {
                    Message = "User doesn't exist",
                    Status = false
                };
            }

            if (user.Role.ToLower() != "student")
            {
                return new Result()
                {
                    Message = "User doesn't have Student Role",
                    Status = false
                };
            }

            var expDto = await _context.StudentCourse_ExpCourses
                .Where(x => x.Student_Course.UserId == user.Id && x.IsCompleted)
                .Select(x => new ExpDto()
                {
                    Idd = x.Exp_Course.Experiment.Idd,
                    Name = x.Exp_Course.Experiment.Name,
                    CourseName = x.Exp_Course.Course.Name,
                    SubmitedAt = x.ReservedDay.ToShortDateString(),
                    Status = x.Status,
                    Grade = x.FinalGrade,
                    FeedBack = x.feedback
                })
                .Where(x => x.Name.ToLower().Contains("" + searchByName.ToLower()) && x.CourseName.ToLower().Contains("" + searchByCourseName.ToLower()))
                .ToListAsync();

            var count = expDto.Count;

            return new Result()
            {
                Data = new
                {
                    result = expDto.Skip(page * 10).Take(10).ToList(),
                    count,
                    next = (page * 10) + 10 >= count ? null : $"https://optimistic-burnell.74-50-88-98.plesk.page/api/Student/Completed?page={page + 1 + 1}",
                    previous = page == 0 ? null : $"https://optimistic-burnell.74-50-88-98.plesk.page/api/Student/Completed?page={page - 1 + 1}"
                },
                Status = true
            };
        }

        public async Task<Result> GetStudentCourses(string email, int page, string searchByName, string searchByCode)
        {
            searchByName += "";
            searchByCode += "";

            if (page == -1)
            {
                page = 0;
            }

            var user = await _unitOfWork.Users.GetByEmail(email);
            if (user == null)
            {
                return new Result()
                {
                    Message = "User doesn't exist",
                    Status = false
                };
            }

            if (user.Role.ToLower() != "student")
            {
                return new Result()
                {
                    Message = "User doesn't have Student Role",
                    Status = false
                };
            }

            var courses = _context.User_Courses.Where(x => x.UserId == user.Id && x.Role == "student").Select(x=>x.Course)
                .Where(x => x.Name.ToLower().Contains("" + searchByName.ToLower()) && x.Code.ToLower().Contains("" + searchByCode.ToLower())).ToList();
            if(!courses.Any())
            {
                return new Result()
                {
                    Message = "User have no assgined courses",
                    Status = false
                };
            }

            var count = courses.Count;
            var coursesPaging = courses.Skip(page * 10).Take(10).ToList();

            var coursesDto = new List<CourseDto>();
            foreach(var course in coursesPaging)
            {
                var courseDto = _mapper.Map<CourseDto>(course);
                coursesDto.Add(courseDto);
            }

            return new Result()
            {
                Data = new
                {
                    result = coursesDto,
                    count,
                    next = (page * 10) + 10 >= count ? null : $"https://stupefied-antonelli.74-50-88-98.plesk.page/api/Course?page={page + 1 + 1}",
                    previous = page == 0 ? null : $"https://stupefied-antonelli.74-50-88-98.plesk.page/api/Course?page={page - 1 + 1}"
                },
                Status = true
            };
        }

        //public async Task<Result> GetStudentResult(string email, Guid courseIdd,Guid expIdd)
        //{
        //    var user = await _unitOfWork.Users.GetByEmail(email);
        //    if (user == null)
        //    {
        //        return new Result()
        //        {
        //            Message = "User doesn't exist",
        //            Status = false
        //        };
        //    }

        //    if (user.Role.ToLower() != "student")
        //    {
        //        return new Result()
        //        {
        //            Message = "User doesn't have Student Role",
        //            Status = false
        //        };
        //    }

        //    var course = await _unitOfWork.Courses.GetByIdd(courseIdd);
        //    if (course == null)
        //        return new Result()
        //        {
        //            Message = "there is no course with this IDD",
        //            Status = false
        //        };

        //    var exp = await _unitOfWork.Experiments.GetByIdd(expIdd);
        //    if (exp == null)
        //        return new Result()
        //        {
        //            Message = "there is no Experiment with this IDD",
        //            Status = false
        //        };

        //    var expCourse = await _context.Exp_Courses
        //        .FirstOrDefaultAsync(x => x.ExperimentId == exp.Id);

        //    var stu_trials = await _context.Student_ExpCourses.Where(x => x.StudentId == user.Id
        //    && x.Exp_CourseId == expCourse.Id)
        //        .Select(x => x.Trials).FirstOrDefaultAsync();

        //    var trialsDto = new List<TrialDto>();
        //    foreach(var stu_trial in stu_trials)
        //    {
        //        var trialDto = _mapper.Map<TrialDto>(stu_trial);
        //        trialDto.LRO_SA = JsonConvert.DeserializeObject<LLO>(stu_trial.LRO_SA);
        //        trialDto.LRO = JsonConvert.DeserializeObject<LLO>(stu_trial.LRO);
        //        trialsDto.Add(trialDto);
        //    }

        //    return new Result()
        //    {
        //        Data = trialsDto,
        //        Status = true
        //    };

        //}


        public async Task<Result> startTrial(Guid expIdd, string studentId)
        {
            var user =  _context.Users.FirstOrDefault(x=>x.IdentityId == Guid.Parse(studentId));
            if (user == null)
            {
                return new Result()
                {
                    Message = "User doesn't exist",
                    Status = false
                };
            }

            if (user.Role.ToLower() != "student")
            {
                return new Result()
                {
                    Message = "User doesn't have Student Role",
                    Status = false
                };
            }

            var exp = await _unitOfWork.Experiments.GetByIdd(expIdd);
            if (exp == null)
                return new Result()
                {
                    Message = "there is no Experiment with this IDD",
                    Status = false
                };

            var courseId = _context.Exp_Courses.FirstOrDefault(x=>x.ExperimentId == exp.Id).CourseId;
            if (courseId == null)
                return new Result()
                {
                    Message = "there is no course with this IDD",
                    Status = false
                };

            var expCourse = await _context.Exp_Courses
                .FirstOrDefaultAsync(x => x.ExperimentId == exp.Id && x.CourseId == courseId);

            if (expCourse == null)
                return new Result()
                {
                    Message = "error expCourse",
                    Status = false
                };

            var studentCourse = await _context.User_Courses
                .FirstOrDefaultAsync(x => x.UserId == user.Id && x.CourseId == courseId);

            if (studentCourse == null)
                return new Result()
                {
                    Message = "error studentCourse",
                    Status = false
                };

            var studentCourse_expCourse = await _context.StudentCourse_ExpCourses
                .FirstOrDefaultAsync(x => x.Exp_CourseId == expCourse.Id && x.Student_CourseId == studentCourse.Id);

            if (studentCourse_expCourse == null)
                return new Result()
                {
                    Message = "error studentCourse_expCourse",
                    Status = false,
                };

            //if(!(studentCourse_expCourse.StartFrom <= DateTime.Now.AddHours(2) && studentCourse_expCourse.EndAt >= DateTime.Now.AddHours(2))) 
            //{
            //    return new Result()
            //    {
            //        Message = "Experiment is not available",
            //        Status = false,
            //    };
            //}

            var studentTrial = new Student_Trial()
            {
                StartedAt= DateTime.Now,
                Status = "started",
                StudentCourse_ExpCourse = studentCourse_expCourse,
                StudentCourse_ExpCourseId = studentCourse_expCourse.Id,
                IsGraded = false,
            };

            if (expCourse.NumbersOfTrials == studentCourse_expCourse.NumberOfTials)
            {
                studentCourse_expCourse.IsCompleted = true;
                studentCourse_expCourse.Status = "Completed";
                await _context.SaveChangesAsync();
            }

            studentCourse_expCourse.NumberOfTials++;

            if(expCourse.NumbersOfTrials < studentCourse_expCourse.NumberOfTials)
            {
                return new Result()
                {
                    Message = "There is no trials left for this Exp",
                    Status = false
                };
            }


            var res = await _context.Trials.AddAsync(studentTrial);

            exp.Editable = false;

            await _context.SaveChangesAsync();

            return new Result()
            {
                Data = new
                {
                    trialId = res.Entity.Id
                },
                Message = "Started Successfully",
                Status = true
            };
        }

        public async Task<Result> SubmitTrial(TrialSubmit submitTrial, string email)
        {
            var user = await _unitOfWork.Users.GetByEmail(email);
            if (user == null)
            {
                return new Result()
                {
                    Message = "User doesn't exist",
                    Status = false
                };
            }

            if (user.Role.ToLower() != "student")
            {
                return new Result()
                {
                    Message = "User doesn't have Student Role",
                    Status = false
                };
            }

            var trial = _context.Trials.FirstOrDefault(x => x.Id == submitTrial.trilId);

            trial.TotalTimeInMin = submitTrial.TotalTimeInMin;
            trial.LLA = JsonConvert.SerializeObject(submitTrial.lla, Formatting.None,
                                        new JsonSerializerSettings
                                        {
                                            NullValueHandling = NullValueHandling.Ignore
                                        });
            trial.SubmitedAt = DateTime.Now;
            trial.Status = "submited";


            var studentCourse_expCourse = _context.StudentCourse_ExpCourses.FirstOrDefault(x=>x.Id == trial.StudentCourse_ExpCourseId);
            if (studentCourse_expCourse == null)
                return new Result()
                {
                    Message = "error studentCourse_expCourse",
                    Status = false,
                };

            var expCourse = _context.Exp_Courses.FirstOrDefault(x => x.Id == studentCourse_expCourse.Exp_CourseId);
            if (expCourse == null)
                return new Result()
                {
                    Message = "error expCourse",
                    Status = false,
                };


            if (expCourse.NumbersOfTrials == studentCourse_expCourse.NumberOfTials)
            {
                studentCourse_expCourse.IsCompleted = true;
                studentCourse_expCourse.Status = "Completed";
            }

            await _unitOfWork.SaveAsync();

            return new Result()
            {
                Message = "Submited Successfully",
                Status = true
            };
        }

        public class TrialSubmit
        {
            public LLO lla { get; set; }
            public Guid trilId { get; set; }
            public float TotalTimeInMin { get; set; }
        }

        //private void AutoGrade(Student_Trial studentSubmint, Experiment exp, int trialsCount)
        //{
        //    var LRO_SA = JsonConvert.DeserializeObject<LLO>(studentSubmint.LLA);
        //    var LLO_MA = JsonConvert.DeserializeObject<LLO>(exp.LLO_MA);
        //    float globalScore = 0;

        //    for (int i = 0; i < LLO_MA.Sections.Count; i++)
        //    {
        //        for (int j = 0; j < LLO_MA.Sections[i].Blocks.Count; j++)
        //        {
        //            if (LLO_MA.Sections[i].Blocks[j].MainType == Common.Enums.MainType.QUESTION)
        //                continue;
        //            LLO_MA.Sections[i].Blocks.RemoveAt(j);
        //        }
        //    }

        //    for (int i = 0; i < LLO_MA.Sections.Count; i++)
        //    {
        //        for (int j = 0; j < LLO_MA.Sections[i].Blocks.Count; j++)
        //        {
        //            if (LLO_MA.Sections[i].Blocks[j].Config.Grading == "1")
        //            {
        //                if (LLO_MA.Sections[i].Blocks[j].Type == Common.Enums.BlockType.OPEN_QUESTION_BLOCK)
        //                {
        //                    studentSubmint.IsGraded = false;
        //                }

        //                else if (LLO_MA.Sections[i].Blocks[j].Type == Common.Enums.BlockType.MULTI_SELECT_MCQ_BLOCK
        //                    || LLO_MA.Sections[i].Blocks[j].Type == Common.Enums.BlockType.SINGLE_SELECT_MCQ_BLOCK
        //                    || LLO_MA.Sections[i].Blocks[j].Type == Common.Enums.BlockType.SINGLE_SELECT_IMAGE_MCQ_BLOCK
        //                    || LLO_MA.Sections[i].Blocks[j].Type == Common.Enums.BlockType.MULTI_SELECT_IMAGE_MCQ_BLOCK)
        //                {
        //                    float totalScore = 0;

        //                    for (int k = 0; k < LRO_SA.Sections[i].Blocks[j].Answers.choices.Count; k++)
        //                    {
        //                        var id = LRO_SA.Sections[i].Blocks[j].Score.choices[k].ChoiceId;
        //                        totalScore += LLO_MA.Sections[i].Blocks[j].Score.choices[id].score;
        //                    }

        //                    LRO_SA.Sections[i].Blocks[j].Score = LLO_MA.Sections[i].Blocks[j].Score;
        //                    LRO_SA.Sections[i].Blocks[j].Config = LLO_MA.Sections[i].Blocks[j].Config;
        //                    LRO_SA.Sections[i].Blocks[j].Content = LLO_MA.Sections[i].Blocks[j].Content;
        //                    LRO_SA.Sections[i].Blocks[j].Answers = LLO_MA.Sections[i].Blocks[j].Answers;

        //                    LRO_SA.Sections[i].Blocks[j].StudentScore = totalScore;
        //                    globalScore += totalScore;
        //                }

        //                else if ((LLO_MA.Sections[i].Blocks[j].Type == Common.Enums.BlockType.TRUE_OR_FALSE_QUESTION_BLOCK))
        //                {
        //                    float totalScore = 0;

        //                    for (int k = 0; k < LRO_SA.Sections[i].Blocks[j].Score.statements.Count; k++)
        //                    {
        //                        var id = LRO_SA.Sections[i].Blocks[j].Score.statements[k].statementId;
        //                        if (LLO_MA.Sections[i].Blocks[j].Score.statements[id].answer == LRO_SA.Sections[i].Blocks[j].Score.statements[k].answer)
        //                        {
        //                            totalScore += LLO_MA.Sections[i].Blocks[j].Score.statements[id].score;
        //                        }

        //                    }

        //                    LRO_SA.Sections[i].Blocks[j].Score = LLO_MA.Sections[i].Blocks[j].Score;
        //                    LRO_SA.Sections[i].Blocks[j].Config = LLO_MA.Sections[i].Blocks[j].Config;
        //                    LRO_SA.Sections[i].Blocks[j].Content = LLO_MA.Sections[i].Blocks[j].Content;
        //                    LRO_SA.Sections[i].Blocks[j].Answers = LLO_MA.Sections[i].Blocks[j].Answers;

        //                    LRO_SA.Sections[i].Blocks[j].StudentScore = totalScore;
        //                    globalScore += totalScore;
        //                }
        //            }

        //        }
        //    }

        //    studentSubmint.LRO = JsonConvert.SerializeObject(LRO_SA, Formatting.None,
        //                                new JsonSerializerSettings
        //                                {
        //                                    NullValueHandling = NullValueHandling.Ignore
        //                                });

        //    studentSubmint.TotalScore = globalScore;
        //    studentSubmint.TrialNumber++;
        //}

        //public async Task<Result> ReserveTimeSlot(string email,Guid expIdd, string courseIdd,int timeSlot)
        //{
        //    var user = await _unitOfWork.Users.GetByEmail(email);
        //    if (user == null)
        //    {
        //        return new Result()
        //        {
        //            Message = "User doesn't exist",
        //            Status = false
        //        };
        //    }

        //    if (user.Role.ToLower() != "student")
        //    {
        //        return new Result()
        //        {
        //            Message = "User doesn't have Student Role",
        //            Status = false
        //        };
        //    }

        //    var course = await _unitOfWork.Courses.GetByIdd(courseIdd);
        //    if (course == null)
        //        return new Result()
        //        {
        //            Message = "there is no course with this IDD",
        //            Status = false
        //        };

        //    var exp = await _unitOfWork.Experiments.GetByIdd(expIdd);
        //    if (exp == null)
        //        return new Result()
        //        {
        //            Message = "there is no Experiment with this IDD",
        //            Status = false
        //        };


        //    var listTimeSlot = await _context.StudentSessions.Where(x => x.TimeSlot == timeSlot).ToListAsync();
        //    var listMachine = await _context.Machines.ToListAsync();

        //    if (listTimeSlot.Count == listMachine.Count)
        //    {
        //        return new Result()
        //        {
        //            Message = "This time Slot is full",
        //            Status = false
        //        };
        //    }

        //    var exp_course = await _context.Exp_Courses.FirstOrDefaultAsync(x => x.CourseId == course.Id && x.ExperimentId == exp.Id);
        //    var checkIfPreUsed = await _context.StudentSessions.FirstOrDefaultAsync(x => x.ExpCourseId == exp_course.Id && x.StudentId == user.Id);

        //    if(checkIfPreUsed != null)
        //    {
        //        return new Result()
        //        {
        //            Message = "You already resrved for this experiment",
        //            Status = false
        //        };
        //    }


        //    var session = new StudentSession()
        //    {
        //        StudentId = user.Id,
        //        Student = user,

        //        ExpCourseId = exp_course.Id,
        //        ExpCourse = exp_course,

        //        TimeSlot = timeSlot
        //    };

        //    var res = await _context.StudentSessions.AddAsync(session);
        //    await _context.SaveChangesAsync();

        //    var studentsExp = new List<StudentExp>();
        //    var students = await _context.StudentSessions.Where(x => x.TimeSlot == timeSlot)
        //                                                 .Select(x => x.Student).ToListAsync();
        //    var studentResources = await _context.StudentSessions.Where(x => x.TimeSlot == timeSlot)
        //                                                         .Select(x=>x.ExpCourse.Resource_Exps.Select(x=>x.Resource).ToList()).ToListAsync();

        //    var machinesExp = new List<MachineExp>();
        //    var machines = await _context.Machines.ToListAsync();
        //    var machineResurces = await _context.Machines.Select(x => x.resource_machines.Select(x => x.Resource).ToList()).ToListAsync();

        //    var resources = await _context.Resources.ToListAsync();

        //    // Pre-Algrothim
        //    for (int i=0; i< students.Count; i++)
        //    {
        //        var listOfIntRes = new List<int>();
        //        for(int j=0; j< studentResources[i].Count ; j++)
        //        {
        //            var id = studentResources[i][j].Id;
        //            listOfIntRes.Add(resources.FindIndex(x => x.Id == id)+1);
        //        }

        //        var studentExp = new StudentExp(i+1, listOfIntRes);
        //        studentsExp.Add(studentExp);
        //    }

        //    for(int i=0; i< machines.Count; i++)
        //    {
        //        var listOfIntRes = new List<int>();
        //        for (int j = 0; j < machineResurces[i].Count; j++)
        //        {
        //            var id = machineResurces[i][j].Id;
        //            listOfIntRes.Add(resources.FindIndex(x => x.Id == id) + 1);
        //        }

        //        var machineExp = new MachineExp(i + 1, listOfIntRes);
        //        machinesExp.Add(machineExp);
        //    }

        //    // Algorithm Code:
        //    MatchGame game = new MatchGame(studentsExp, machinesExp);
        //    var resMachines = game.GetFinalResult();

        //    foreach (var ress in resMachines)
        //    {
        //        try
        //        {
        //            var studentSession = await _context.StudentSessions.FirstOrDefaultAsync(x => x.StudentId == students[ress.Partner.Id-1].Id && x.TimeSlot == timeSlot);
        //            var id = machines[ress.Id-1].Id;
        //            studentSession.MachineId = id;
        //            studentSession.Machine = await _context.Machines.FindAsync(id);
        //        }
        //        catch(Exception e)
        //        {

        //        }
        //    }


        //    await _context.SaveChangesAsync();

        //    return new Result()
        //    {
        //        Data = $"you have been successfully resrved Time Slot:{timeSlot}",
        //        Status = true
        //    };

        //}
    }
}
