using AdnTestingSystem.Services.Interfaces;
using AdnTestingSystem.Services.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AdnTestingSystem.Api.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class BlogController : ControllerBase
    {
        private readonly IBlogService _service;
        public BlogController(IBlogService service)
        {
            _service = service;
        }

        /// <summary>
        /// Lấy danh sách blogs
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAll();
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Tạo blog mới
        /// </summary>
        /// <param name="request">Dữ liệu blog cần tạo</param>
        /// <returns>Kết quả tạo blog</returns>
        [HttpPost("new-blog")]
        public async Task<IActionResult> CreateBlog([FromBody] CreateBlogRequest request)
        {
            var result = await _service.CreateBlogAsync(request);
            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }
    }
}
