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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public async Task<Result> GetAllCourses()
        {
            var courses = await _unitOfWork.Courses.GetAll();

            if(!courses.Any())
            {
                return new Result()
                {
                    Status = true,
                    Message = "There is no Courses"
                };
            }

            var CourseListDto = new List<CourseDto>();
            foreach(var course in courses)
            {
                var courseDtp = _iMapper.Map<CourseDto>(course);
                CourseListDto.Add(courseDtp);
            }

            return new Result()
            {
                Status = true,
                Data = CourseListDto
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
                Role = role
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
                Status = true,
                Message = $"{user.Email}: has been added to the Course Successfully with Role {role}"
            };
        }

        public async Task<Result> GetUsersAssignedToCourse(Guid idd, string role)
        {
            var course = await _unitOfWork.Courses.GetByIdd(idd);
            if (course == null)
            {
                return new Result()
                {
                    Status = false,
                    Message = "Course doesn't exists"
                };
            }

            var userListDto = await _context.User_Courses
                        .Where(x => x.CourseId == course.Id && x.Role == role.ToLower())
                        .Select(x => _iMapper.Map<UserDto>(x.User)).ToListAsync();

            if (!userListDto.Any())
            {
                return new Result()
                {
                    Status = false,
                    Message = "There is no User with this Role"
                };
            }

            return new Result()
            {
                Status = true,
                Data = userListDto
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

            var exp = await _unitOfWork.Experiments.GetByIdd(expIdd);
            if (exp == null)
                return new Result()
                {
                    Status = false,
                    Message = "Expirment doesn't exists or it already "
                };


            //Allow add same exp with copy or only allow 1 exp with no Origin
            var expCopy = exp;
            expCopy.Id = Guid.NewGuid();
            expCopy.Idd = Guid.NewGuid();
            expCopy.AddedDate = DateTime.Now;
            expCopy.UpdateDate = DateTime.Now;
            expCopy.Active = false;
            

            await _unitOfWork.Experiments.Create(expCopy);

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
                Data = "Experiment has been added to the Course successfully"
            };
        }

        public async Task<Result> GetExpAssignedToCourse(Guid idd)
        {
            var course = await _unitOfWork.Courses.GetByIdd(idd);
            if (course == null)
            {
                return new Result()
                {
                    Status = false,
                    Message = "Course doesn't exists"
                };
            }

            var expListDto = await _context.Exp_Courses
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
                    .ToListAsync();
           


            if (!expListDto.Any())
            {
                return new Result()
                {
                    Status = true,
                    Message = "There is no Expirment in this Course"
                };
            }

            return new Result()
            {
                Status = true,
                Data = expListDto
            };
        }
    }
}
