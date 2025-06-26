using AdnTestingSystem.Repositories.UnitOfWork;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AdnTestingSystem.Api.Middlewares
{
    public class JwtSessionValidationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _config;

        public JwtSessionValidationMiddleware(RequestDelegate next, IConfiguration config)
        {
            _next = next;
            _config = config;
        }

        public async Task Invoke(HttpContext context, IUnitOfWork uow)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (!string.IsNullOrEmpty(token))
            {
                try
                {
                    var tokenHandler = new JwtSecurityTokenHandler();
                    var key = Encoding.UTF8.GetBytes(_config["Jwt:Key"]!);

                    tokenHandler.ValidateToken(token, new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = _config["Jwt:Issuer"],
                        ValidAudience = _config["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateLifetime = false
                    }, out SecurityToken validatedToken);

                    var jwtToken = (JwtSecurityToken)validatedToken;

                    var userId = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                    var sessionId = jwtToken.Claims.FirstOrDefault(c => c.Type == "sessionId")?.Value;

                    if (userId != null && sessionId != null)
                    {
                        var user = await uow.Users.GetAsync(u => u.Id.ToString() == userId);

                        if (user == null || user.RefreshToken != sessionId)
                        {
                            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                            await context.Response.WriteAsync("Phiên đăng nhập không hợp lệ (đăng nhập ở thiết bị khác?)");
                            return;
                        }

                        if (user.RefreshTokenExpiryTime <= DateTime.UtcNow)
                        {
                            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                            await context.Response.WriteAsync("Phiên đăng nhập đã hết hạn (sau 24 giờ)");
                            return;
                        }
                    }
                }
                catch
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync("Token không hợp lệ");
                    return;
                }
            }

            await _next(context);
        }
    }
}
