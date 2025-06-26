using AdnTestingSystem.Repositories.Models;
using AdnTestingSystem.Repositories.UnitOfWork;
using AdnTestingSystem.Services.Interfaces;
using AdnTestingSystem.Services.Responses;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdnTestingSystem.Services.Services
{
    public class BlogService : IBlogService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public BlogService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<CommonResponse<IEnumerable<BlogResponse>>> GetAll()
        {
            var blogs = await _unitOfWork.Blogs
                .Query()
                .Include(b => b.Author)
                .ThenInclude(a => a.Profile)
                .ToListAsync();

            var blogResponses = _mapper.Map<IEnumerable<BlogResponse>>(blogs);

            return CommonResponse<IEnumerable<BlogResponse>>.Ok(blogResponses);
        }


    }
}
