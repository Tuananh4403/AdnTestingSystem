using AdnTestingSystem.Repositories.Models;
using AdnTestingSystem.Repositories.UnitOfWork;
using AdnTestingSystem.Services.Interfaces;
using AdnTestingSystem.Services.Requests;
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

        public async Task<CommonResponse<string>> CreateBlogAsync(CreateBlogRequest request)
        {
            var author = await _unitOfWork.Users.GetAsync(u => u.Id == request.AuthorId);
            if (author == null)
                return CommonResponse<string>.Fail("Không tìm thấy tác giả.");

            var blog = new Blog
            {
                Title = request.Title,
                Content = request.Content,
                PublishedAt = request.PublishedAt,
                AuthorId = request.AuthorId
            };

            await _unitOfWork.Blogs.AddAsync(blog);
            await _unitOfWork.CompleteAsync();

            return CommonResponse<string>.Ok("Tạo blog thành công.");
        }



    }
}
