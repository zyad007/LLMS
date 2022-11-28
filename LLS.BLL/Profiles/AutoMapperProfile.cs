using AutoMapper;
using LLS.Common.Dto;
using LLS.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLS.BLL.Profiles
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<UserDto, User>().ReverseMap();

            CreateMap<TrialDto, Student_Trial>().ReverseMap()
                .ForMember(dest => dest.LRO, act => act.Ignore())
                .ForMember(dest => dest.LRO_SA, act => act.Ignore());


            CreateMap<ExpDto, Experiment>().ReverseMap()
                .ForMember(dest => dest.LLO, act => act.Ignore());

            CreateMap<CourseDto, Course>().ReverseMap();
        }

    }
}
