using AdnTestingSystem.Services.Interfaces;
using AdnTestingSystem.Services.Requests;
using Microsoft.AspNetCore.Mvc;

namespace AdnTestingSystem.Api.Controllers
{
    [ApiController]
    [Route("api/auth")]
    [Produces("application/json")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Đăng ký tài khoản người dùng mới.
        /// </summary>
        /// <param name="request">Thông tin đăng ký</param>
        /// <returns>Kết quả đăng ký và yêu cầu xác thực email</returns>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var result = await _authService.RegisterAsync(request);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Xác thực email bằng mã đã gửi trong email.
        /// </summary>
        /// <param name="request">Email và mã xác thực</param>
        /// <returns>Kết quả xác thực email</returns>
        [HttpPost("verify-email")]
        public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailRequest request)
        {
            var result = await _authService.VerifyEmailAsync(request);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Đăng nhập hệ thống bằng email và mật khẩu.
        /// </summary>
        /// <param name="request">Thông tin đăng nhập</param>
        /// <returns>Token JWT nếu đăng nhập thành công</returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var result = await _authService.LoginAsync(request);
            return result.Success ? Ok(result) : Unauthorized(result);
        }
    }
}
