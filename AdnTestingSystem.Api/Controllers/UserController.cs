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
            var result = await _userService.GetAllUsersAsync(page, pageSize);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Lấy danh sách nhân viên (Staff) theo tên (nếu có).
        /// </summary>
        [HttpGet("staffs")]
        [Authorize(Roles = "Admin,Manager,Staff")]
        public async Task<IActionResult> GetStaffList([FromQuery] string? search)
        {
            var result = await _userService.GetStaffListAsync(search);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetMyProfile()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized();

            int userId = int.Parse(userIdClaim);

            var user = await _uow.Users.GetAsync(u => u.Id == userId, includeProperties: "Profile");

            if (user == null || user.Profile == null)
                return NotFound(CommonResponse<string>.Fail("Không tìm thấy hồ sơ."));

            var dto = new UserProfileDto
            {
                Id = user.Id,
                Email = user.Email,
                FullName = user.Profile.FullName,
                Phone = user.Profile.Phone,
                Address = user.Profile.Address,
                Gender = user.Profile.Gender,
                DateOfBirth = user.Profile.DateOfBirth?.ToString("dd-MM-yyyy"),
                AvatarPath = user.Profile.AvatarPath
            };

            return Ok(CommonResponse<UserProfileDto>.Ok(dto, "Lấy thông tin người dùng thành công."));
        }
        [HttpPut("me/profile")]
        public async Task<IActionResult> UpdateMyProfile([FromForm] UpdateUserProfileRequest request)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var user = await _uow.Users.GetAsync(u => u.Id == userId, includeProperties: "Profile");

            if (user?.Profile == null)
                return NotFound(CommonResponse<string>.Fail("Không tìm thấy hồ sơ."));

            user.Profile.FullName = request.FullName;
            user.Profile.Phone = request.Phone;
            user.Profile.Address = request.Address;
            user.Profile.Gender = request.Gender;
            user.Profile.DateOfBirth = DateTime.TryParse(request.DateOfBirth, out var dob) ? dob : null;

            if (request.Avatar != null && request.Avatar.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "avatars");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var fileName = $"avatar_user_{userId}_{Guid.NewGuid():N}{Path.GetExtension(request.Avatar.FileName)}";
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await request.Avatar.CopyToAsync(stream);
                }

                user.Profile.AvatarPath = $"/uploads/avatars/{fileName}";
            }

            _uow.Users.Update(user);
            await _uow.CompleteAsync();

            return Ok(CommonResponse<string>.Ok("Cập nhật thành công"));
        }

        
    }
}
