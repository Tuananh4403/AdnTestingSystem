using AdnTestingSystem.Repositories.Models;
using AdnTestingSystem.Repositories.UnitOfWork;
using AdnTestingSystem.Services.Interfaces;
using AdnTestingSystem.Services.Requests;
using AdnTestingSystem.Services.Responses;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;


namespace AdnTestingSystem.Services.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _uow;
        private readonly IConfiguration _config;
        private readonly IEmailSender _emailSender;

        public AuthService(IUnitOfWork uow, IConfiguration config, IEmailSender emailSender)
        {
            _uow = uow;
            _config = config;
            _emailSender = emailSender;
        }

        public async Task<CommonResponse<string>> RegisterAsync(RegisterRequest request)
        {
            var existing = await _uow.Users.GetAsync(u => u.Email == request.Email);
            if (existing != null)
                return CommonResponse<string>.Fail("Email đã được sử dụng.");

            var hash = BCrypt.Net.BCrypt.HashPassword(request.Password);
            var code = Guid.NewGuid().ToString().Substring(0, 6);

            var user = new User
            {
                Email = request.Email,
                PasswordHash = hash,
                EmailVerificationCode = code,
                VerificationCodeExpiresAt = DateTime.UtcNow.AddMinutes(10),
                IsEmailConfirmed = false,
                Role = UserRole.Customer,
                Profile = new UserProfile
                {
                    FullName = request.FullName,
                    Phone = request.Phone,
                    Address = request.Address,
                    DateOfBirth = request.DateOfBirth
                }
            };

            await _uow.Users.AddAsync(user);
            await _uow.CompleteAsync();

            await _emailSender.SendAsync(user.Email, "Xác thực tài khoản", $"Mã xác thực: {code}");

            return CommonResponse<string>.Ok(user.Email, "Đăng ký thành công. Vui lòng xác thực email.");
        }

        public async Task<CommonResponse<string>> VerifyEmailAsync(VerifyEmailRequest request)
        {
            var user = await _uow.Users.GetAsync(u => u.Email == request.Email);
            if (user == null || user.EmailVerificationCode != request.Code || user.VerificationCodeExpiresAt < DateTime.UtcNow)
                return CommonResponse<string>.Fail("Mã xác thực không hợp lệ hoặc đã hết hạn");

            user.IsEmailConfirmed = true;
            user.EmailVerificationCode = null;
            user.VerificationCodeExpiresAt = null;

            _uow.Users.Update(user);
            await _uow.CompleteAsync();

            return CommonResponse<string>.Ok(user.Email, "Email xác thực thành công.");
        }

        public async Task<CommonResponse<string>> LoginAsync(LoginRequest request)
        {
            var user = await _uow.Users.GetAsync(u => u.Email == request.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                return CommonResponse<string>.Fail("Email hoặc mật khẩu không đúng");

            if (!user.IsEmailConfirmed)
                return CommonResponse<string>.Fail("Tài khoản chưa được xác thực email");

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role.ToString()),
                new Claim(ClaimTypes.Name, Uri.EscapeDataString(user.Profile?.FullName ?? "")),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds
            );

            return CommonResponse<string>.Ok(new JwtSecurityTokenHandler().WriteToken(token), "Đăng nhập thành công");
        }
    }

}
