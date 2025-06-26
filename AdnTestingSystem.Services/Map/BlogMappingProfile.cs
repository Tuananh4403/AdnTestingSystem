using AdnTestingSystem.Repositories.Models;
using AdnTestingSystem.Services.Responses;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdnTestingSystem.Services.Map
{
    public class BlogMappingProfile : Profile
    {
        public BlogMappingProfile()
        {
            CreateMap<Blog, BlogResponse>()
                .ForMember(dest => dest.FullName,
                           opt => opt.MapFrom(src => src.Author.Profile.FullName));
        }
    }

}
