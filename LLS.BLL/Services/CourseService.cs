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
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace LLS.BLL.Services
{
    public class CourseService : ICourseService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _iMapper;
        private readonly AppDbContext _context;
        public CourseService(IUnitOfWork unitOfWork,
                             IMapper iMapper,
                             AppDbContext context)
        {
            _unitOfWork = unitOfWork;
            _iMapper = iMapper;
            _context = context;
        }

        public async Task<Result> CreateCourse(CourseDto courseDto)
        {
            var exist = await _unitOfWork.Courses.GetByIdd(courseDto.Idd);
            if(exist != null)
            {
                return new Result()
                {
                    Status = false,
                    Message = "The Course IDD already in use"
                };
            }

            var course = _iMapper.Map<Course>(courseDto);

            var res = await _unitOfWork.Courses.Create(course);
            if(res == false)
            {
                return new Result()
                {
                    Status = false,
                    Message = "Something went wrong"
                };
            }

            await _unitOfWork.SaveAsync();

            return new Result()
            {
                Status = true,
                Data = courseDto,
                Message = "Course Added Successfully"
            };
        }

        public async Task<Result> DeleteCourse(Guid idd)
        {
            var exist = await _unitOfWork.Courses.GetByIdd(idd);
            if (exist == null)
            {
                return new Result()
                {
                    Status = false,
                    Message = "The Course doesn't exixsts"
                };
            }

            _unitOfWork.Courses.Delete(exist);
            await _unitOfWork.SaveAsync();

            return new Result()
            {
                Status = true,
                Data = "Deleted Successfully"
            };
        }

        public async Task<Result> GetAllCourses(int page, string searchByName, string searchByCode)
        {
            //var courses = await _unitOfWork.Courses.GetAll();

            searchByCode += "";
            searchByName += "";

            if(page == -1 )
            {
                page = 0;
            }

            var courses = _context.Courses
                .Where(x => x.Name.ToLower().Contains("" + searchByName.ToLower()) && x.Code.ToLower().Contains("" + searchByCode.ToLower()))
                .Skip(page*10).Take(10).ToList();

            var count = _context.Courses.Count();

            var CourseListDto = new List<CourseDto>();
            foreach(var course in courses)
            {
                var courseDtp = _iMapper.Map<CourseDto>(course);
                CourseListDto.Add(courseDtp);
            }

            return new Result()
            {
                Status = true,
                Data = new 
                {
                    result = CourseListDto,
                    count,
                    next = (page*10)+10 >= count ? null : $"https://optimistic-burnell.74-50-88-98.plesk.page/api/Course?page={page+1+1}",
                    previous = page == 0 ? null : $"https://optimistic-burnell.74-50-88-98.plesk.page/api/Course?page={page-1+1}"
                }
            };
        }

        public async Task<Result> GetCourse(Guid idd)
        {
            var exist = await _unitOfWork.Courses.GetByIdd(idd);
            if (exist == null)
            {
                return new Result()
                {
                    Status = false,
                    Message = "The Course doesn't exixsts"
                };
            }

            return new Result()
            {
                Status = true,
                Data = _iMapper.Map<CourseDto>(exist)
            };
        }

        public async Task<Result> UpdateCourse(CourseDto courseDto)
        {
            var exist = await _unitOfWork.Courses.GetByIdd(courseDto.Idd);
            if (exist == null)
            {
                return new Result()
                {
                    Status = false,
                    Message = "The Course doesn't exixsts"
                };
            }

            var course = _iMapper.Map<Course>(courseDto);

            await _unitOfWork.Courses.Update(course);
            await _unitOfWork.SaveAsync();

            return new Result()
            {
                Status = true,
                Message = "Updated Successfully",
                Data = courseDto
            };
        }

        //

        public async Task<Result> AssignUserToCourse(Guid userIdd, Guid idd,string role)
        {
            var course = await _unitOfWork.Courses.GetByIdd(idd);
            if (course == null)
                return new Result()
                {
                    Status = false,
                    Message = "Course doesn't exists"
                };

            var user = await _unitOfWork.Users.GetByIdd(userIdd);
            if (user == null)
                return new Result()
                {
                    Status = false,
                    Message = "User doesn't exists"
                };

            if (user.Role.ToLower() != role.ToLower())
                return new Result()
                {
                    Status = false,
                    Message = $"{user.Email}: must have the Role {role} to be Assigned as A {role}"
                };

            var check = await _context.User_Courses.FirstOrDefaultAsync(x => x.CourseId == course.Id && x.UserId == user.Id);
            if (check != null)
            {
                return new Result()
                {
                    Status = false,
                    Message = $"{user.Email}: is already assigned to the course"
                };
            }

            var user_course = new User_Course()
            {
                UserId = user.Id,
                User = user,
                CourseId = course.Id,
                Course = course,
                Role = role,
                Update = DateTime.Now
            };

            var result = await _context.User_Courses.AddAsync(user_course);
            if (result == null)
                return new Result()
                {
                    Status = false,
                    Message = $"{user.Email}:Something went wrong"
                };

            if(role.ToLower() == "student")
            {
                var exp_courses = await _context.Exp_Courses.Where(x => x.CourseId == course.Id)
                                            .ToListAsync();

                foreach (var exp_course in exp_courses)
                {
                    var studentCourse_expCourse = new StudentCourse_ExpCourse()
                    {
                        Student_CourseId = user_course.Id,
                        Student_Course = user_course,
                        Exp_CourseId = exp_course.Id,
                        Exp_Course = exp_course
                    };

                    await _context.StudentCourse_ExpCourses.AddAsync(studentCourse_expCourse);
                }


            }

            course.NumberOfStudents++;

            await _unitOfWork.SaveAsync();

            return new Result()
            {
                Data = _iMapper.Map<UserDto>(user),
                Status = true,
                Message = $"{user.Email}: has been added to the Course Successfully with Role {role}"
            };
        }

        public async Task<Result> GetUsersAssignedToCourse(Guid idd, string role, int page, string search)
        {
            search += "";

            if (page == -1)
            {
                page = 0;
            }

            var course = await _unitOfWork.Courses.GetByIdd(idd);
            if (course == null)
            {
                return new Result()
                {
                    Status = false,
                    Message = "Course doesn't exists"
                };
            }

            var userListDto = _context.User_Courses
                        .Where(x => x.CourseId == course.Id && x.Role == role.ToLower())
                        .Select(x => new UserDto()
                        {
                            Update = x.Update,
                            Email = x.User.Email,
                            Role = x.User.Role,
                            FirstName = x.User.FirstName,
                            Lastname = x.User.Lastname,
                            Idd = x.User.Idd
                        }
                        ).ToList();


            var users = userListDto.Where(x => x.Email.ToLower().Contains("" + search.ToLower())).Skip(page*10).Take(10).ToList();
            int count = userListDto.Count;

            return new Result()
            {
                Status = true,
                Data = new
                {
                    result = users,
                    count,
                    next = (page * 10) + 10 >= count ? null : $"https://optimistic-burnell.74-50-88-98.plesk.page/api/Course/{idd}/{role}?page={page + 1+1}",
                    previous = page == 0 ? null : $"https://optimistic-burnell.74-50-88-98.plesk.page/api/Course/{idd}/{role}?page={page - 1+1}"
                }
            };
        }

        public async Task<Result> AssignExpToCourse(Guid expIdd, Guid courseIdd, DateTime startDate, DateTime endDate,int trials)
        {
            var course = await _unitOfWork.Courses.GetByIdd(courseIdd);
            if (course == null)
            {
                return new Result()
                {
                    Status = false,
                    Message = "Course doesn't exists"
                };
            }

            var exp = await _context.Expirments.FirstOrDefaultAsync(x=>x.Idd == expIdd);
            if (exp == null)
                return new Result()
                {
                    Status = false,
                    Message = "Expirment doesn't exists or it already "
                };


            //Allow add same exp with copy or only allow 1 exp with no Origin
            var expCopy = new Experiment();
            expCopy.Id = Guid.NewGuid();
            expCopy.Idd = Guid.NewGuid();
            expCopy.AddedDate = DateTime.Now;
            expCopy.UpdateDate = DateTime.Now;
            expCopy.Active = false;

            expCopy.Name = exp.Name;
            expCopy.Description = exp.Description;
            expCopy.AuthorName = exp.AuthorName;
            expCopy.AuthorId = exp.AuthorId;
            expCopy.LLO = exp.LLO;
            expCopy.Editable = true;
            expCopy.hasLLO = exp.hasLLO;
            

            await _unitOfWork.Experiments.Create(expCopy);

            exp.RelatedCourse = course.Name;

            var exp_ress = await _context.Resource_Exps.Where(x => x.ExperimentId == exp.Id).ToListAsync();
            foreach(var exp_resTemp in exp_ress)
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

            //var check = await _context.Exp_Courses.FirstOrDefaultAsync(x => x.CourseId == course.Id && x.Experiment.Id == exp.Id);
            //if (check != null)
            //{
            //    return new Result()
            //    {
            //        Status = false,
            //        Message = "This Experiment is already assigned to the course"
            //    };
            //}

            var exp_Course = new Exp_Course()
            {
                ExperimentId = expCopy.Id,
                Experiment = expCopy,
                CourseId = course.Id,
                Course = course,
                StartDate = startDate,
                EndDate = endDate,
                NumbersOfTrials = trials
            };

            
            var result = await _context.Exp_Courses.AddAsync(exp_Course);
            if (result == null)
            {
                return new Result()
                {
                    Status = false,
                    Message = "Something went wrong"
                };
            }

            var students = await _context.User_Courses.Where(x => x.CourseId == course.Id && x.Role == "student")
                                .Select(x => x.User).ToListAsync();

            foreach (var student in students)
            {
                var studentCourse = await _context.User_Courses.FirstOrDefaultAsync(x => x.UserId == student.Id && x.CourseId == course.Id);

                var student_expCourse = new StudentCourse_ExpCourse()
                {
                    Student_CourseId = studentCourse.Id,
                    Student_Course = studentCourse,
                    Exp_Course = result.Entity,
                    Exp_CourseId = result.Entity.Id
                };

                await _context.StudentCourse_ExpCourses.AddAsync(student_expCourse);
            }

            course.NumberOfExp++;

            await _unitOfWork.SaveAsync();
            return new Result()
            {
                Status = true,
                Data = new
                {
                    idd = expCopy.Idd,
                    name = expCopy.Name,
                    description = expCopy.Description,
                    startDate,
                    endDate,
                    NumbersOfTrials = trials
                }
                            
            };
        }

        public async Task<Result> GetExpAssignedToCourse(Guid idd, int page, string search)
        {
            search += "";

            if (page == -1)
            {
                page = 0;
            }

            var course = await _unitOfWork.Courses.GetByIdd(idd);
            if (course == null)
            {
                return new Result()
                {
                    Status = false,
                    Message = "Course doesn't exists"
                };
            }

            var expListDto = _context.Exp_Courses
                    .Where(x => x.CourseId == course.Id)
                    .Select(x => new ExpDto()
                    {
                        Name = x.Experiment.Name,
                        Idd = x.Experiment.Idd,
                        AuthorName = x.Experiment.AuthorName,
                        Description = x.Experiment.Description,
                        StartDate = x.StartDate,
                        EndDate = x.EndDate,
                        UpdateDate = x.Experiment.UpdateDate,
                        CourseIdd = course.Idd,
                        CourseName = course.Name
                    })
                    .ToList();

            var exps = expListDto.Where(x => x.Name.ToLower().Contains("" + search.ToLower())).Skip(page * 10).Take(10).ToList();
            int count = expListDto.Count;

            return new Result()
            {
                Status = true,
                Data = new
                {
                    result = exps,
                    count,
                    next = (page * 10) + 10 >= count ? null : $"https://optimistic-burnell.74-50-88-98.plesk.page/api/Course/{idd}/Assign-Experiment?page={page + 1+1}",
                    previous = page == 0 ? null : $"https://optimistic-burnell.74-50-88-98.plesk.page/api/Course/{idd}/Assign-Experiment?page={page - 1+1}"
                
                }
            };
        }
    }
}
