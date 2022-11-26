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

        public async Task<Result> DeleteCourse(string idd)
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

        public async Task<Result> GetCourse(string idd)
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

        public async Task<Result> AssignUserToCourse(string email, string idd,string role)
        {
            var course = await _unitOfWork.Courses.GetByIdd(idd);
            if (course == null)
                return new Result()
                {
                    Status = false,
                    Message = "Course doesn't exists"
                };

            var user = await _unitOfWork.Users.GetByEmail(email);
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
                    Message = $"User must have the Role {role} to be Assigned as A {role}"
                };

            var check = await _context.User_Courses.FirstOrDefaultAsync(x => x.CourseId == course.Id && x.UserId == user.Id);
            if (check != null)
            {
                return new Result()
                {
                    Status = false,
                    Message = "This User is already assigned to the course"
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
                    Message = "Something went wrong"
                };

            if(role.ToLower() == "student")
            {
                var exp_courses = await _context.Exp_Courses.Where(x => x.CourseId == course.Id)
                                            .ToListAsync();

                foreach (var exp_course in exp_courses)
                {
                    var student_expCourse = new Student_ExpCourse()
                    {
                        StudentId = user.Id,
                        User = user,
                        Exp_CourseId = exp_course.Id,
                        Exp_Course = exp_course
                    };

                    await _context.Student_ExpCourses.AddAsync(student_expCourse);
                }


                course.NumberOfStudents++;
            }

            await _unitOfWork.SaveAsync();

            return new Result()
            {
                Status = true,
                Message = $"User has been added to the Course Successfully with Role {role}"
            };
        }

        public async Task<Result> GetUsersAssignedToCourse(string idd, string role)
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

        public async Task<Result> AssignExpToCourse(Guid expIdd, string courseIdd, DateTime startDate, DateTime endDate,int trials, List<Guid> resourcesId)
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

            var check = await _context.Exp_Courses.FirstOrDefaultAsync(x => x.CourseId == course.Id && x.Experiment.Id == exp.Id);
            if (check != null)
            {
                return new Result()
                {
                    Status = false,
                    Message = "This Expirment is already assigned to the course"
                };
            }

            var exp_Course = new Exp_Course()
            {
                ExperimentId = exp.Id,
                Experiment = exp,
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

            //var resource_Exps = new List<Resource_Exp>();
            //foreach (var resourceId in resourcesId)
            //{
            //    var resource_exp = new Resource_Exp()
            //    {
            //        Exp_Course = result.Entity,
            //        Exp_CourseId = result.Entity.Id,
            //        ResourceId = resourceId,
            //        Resource = _context.Resources.Find(resourceId)
            //    };
            //    resource_Exps.Add(resource_exp);
            //}

            //await _context.Resource_Exps.AddRangeAsync(resource_Exps);

            var students = await _context.User_Courses.Where(x => x.CourseId == course.Id && x.Role == "student")
                                .Select(x => x.User).ToListAsync();

            foreach (var student in students)
            {
                var student_expCourse = new Student_ExpCourse()
                {
                    StudentId = student.Id,
                    Exp_Course = result.Entity,
                    Exp_CourseId = result.Entity.Id,
                    User = student
                };

                await _context.Student_ExpCourses.AddAsync(student_expCourse);
            }


            course.NumberOfExp++;

            await _unitOfWork.SaveAsync();
            return new Result()
            {
                Status = true,
                Data = "Experiment has been added to the Course successfully"
            };
        }

        public async Task<Result> GetExpAssignedToCourse(string idd)
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
            var exp_courses = await _context.Exp_Courses.Where(x => x.CourseId == course.Id).ToListAsync();

            var expListDto = await _context.Exp_Courses
                    .Where(x => x.CourseId == course.Id)
                    .Select(x => new ExpDto()
                    {
                        Name = x.Experiment.Name,
                        Idd = x.Experiment.Idd,
                        AuthorName = x.Experiment.AuthorName,
                        AuthorId = x.Experiment.AuthorId,
                        CourseName = course.Name,
                        CourseIdd = course.Idd,
                        Description = x.Experiment.Description,
                        LLO = JsonConvert.DeserializeObject<LLO>(x.Experiment.LLO),
                        LLO_MA = JsonConvert.DeserializeObject<LLO>(x.Experiment.LLO_MA),
                        StartDate = x.StartDate,
                        EndDate = x.EndDate,
                        //Resources = x.Resource_Exps.Select(x => x.Resource).ToList()
                    })
                    .ToListAsync();
           


            if (!expListDto.Any())
            {
                return new Result()
                {
                    Status = false,
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
