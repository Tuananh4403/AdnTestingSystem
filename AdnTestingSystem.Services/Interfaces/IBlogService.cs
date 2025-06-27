using AdnTestingSystem.Repositories.Models;
using AdnTestingSystem.Services.Requests;
using AdnTestingSystem.Services.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdnTestingSystem.Services.Interfaces
{
    public interface IBlogService
    {
        Task<CommonResponse<IEnumerable<BlogResponse>>> GetAll();
        Task<CommonResponse<string>> CreateBlogAsync(CreateBlogRequest request);
    }
}
