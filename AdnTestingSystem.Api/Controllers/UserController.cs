using AdnTestingSystem.Repositories.Models;
using AdnTestingSystem.Repositories.UnitOfWork;
using AdnTestingSystem.Services.Helpers;
using AdnTestingSystem.Services.Interfaces;
using AdnTestingSystem.Services.Requests;
using AdnTestingSystem.Services.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AdnTestingSystem.Api.Controllers
{
    [ApiController]
    [Route("api/users")]
    [Produces("application/json")]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUnitOfWork _uow;
        private readonly IUserManagementService _userService;

        public UserController(IUnitOfWork uow, IUserManagementService userService)
        {
            _uow = uow;
            _userService = userService;
        }

        /// <summary>
        /// Lấy thông tin hồ sơ người dùng hiện tại.
        /// </summary>
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var user = await _uow.Users.GetAsync(u => u.Id == userId, includeProperties: "Profile");

            if (user == null || user.Profile == null)
                return NotFound(CommonResponse<string>.Fail("Không tìm thấy hồ sơ."));

            return Ok(CommonResponse<UserProfile>.Ok(user.Profile));
        }

        /// <summary>
        /// Admin tạo tài khoản người dùng mới (có thể chọn vai trò).
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
        {
            var result = await _userService.CreateUserAsync(request);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Lấy danh sách tất cả người dùng (Admin).
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllUsers([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            var query = _uow.Users.Query().Where(u => u.IsEmailConfirmed);
            var paged = await PaginationHelper.ToPagedResultAsync(query, page, pageSize);
            return Ok(CommonResponse<object>.Ok(paged));
        }
    }
}
