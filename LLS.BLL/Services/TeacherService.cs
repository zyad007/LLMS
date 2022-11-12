using AutoMapper;
using LLS.BLL.IServices;
using LLS.Common.Dto;
using LLS.Common.Transfere_Layer_Object;
using LLS.DAL.Data;
using LLS.DAL.Interfaces;
using System;
using System.Collections.Generic;
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

        public async Task<Result> GetTeacherCourses(string email)
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

            if (user.Role.ToLower() != "teacher")
            {
                return new Result()
                {
                    Message = "User doesn't have Teacher Role",
                    Status = false
                };
            }

            var courses = _context.User_Courses.Where(x => x.UserId == user.Id && x.Role == "teacher").Select(x=>x.Course).ToList();

            var coursesDto = new List<CourseDto>();
            foreach (var course in courses)
            {
                var courseDto = _mapper.Map<CourseDto>(course);
                coursesDto.Add(courseDto);
            }

            return new Result()
            {
                Data = coursesDto,
                Status = true
            };
        }
    }
}
