using eBooking.Core.CustomExceptions;
using eBooking.Core.Utils;
using eBooking.Core.Base;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AdnTestingSystem.ServiceDefaults;

public static class Authentication
{
    public static string GenerateToken(string userId, string role, string? firebaseUid, bool isRefreshToken, JwtSettings jwtSettings)
    {
        if (string.IsNullOrEmpty(jwtSettings.SecretKey))
        {
            throw new ArgumentException("JWT SecretKey is missing in configuration.");
        }

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim("Id", userId),
            new Claim(ClaimTypes.NameIdentifier, userId),
            new Claim(ClaimTypes.Role, role),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), 
            new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64) 
        };

        if (!string.IsNullOrEmpty(firebaseUid))
        {
            claims.Add(new Claim("FirebaseUid", firebaseUid));
        }

        if (isRefreshToken)
        {
            claims.Add(new Claim("isRefreshToken", "true"));
        }

        var expires = isRefreshToken
            ? DateTime.UtcNow.AddDays(jwtSettings.RefreshTokenExpirationDays)
            : DateTime.UtcNow.AddMinutes(jwtSettings.AccessTokenExpirationMinutes);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Issuer = jwtSettings.Issuer,
            Audience = jwtSettings.Audience,
            Subject = new ClaimsIdentity(claims),
            Expires = expires,
            SigningCredentials = credentials
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public static string GenerateAccessToken(string userId, string role, JwtSettings jwtSettings)
    {
        var keyString = jwtSettings.SecretKey
                        ?? throw new ErrorException(StatusCodes.Status401Unauthorized, ErrorCode.UnAuthorized,
                            "JWT key is not configured.");
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));

        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new("Id", userId),
            new(ClaimTypes.Role, role),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new("isRefreshToken", "false")
        };
        var token = new JwtSecurityToken(
            issuer: jwtSettings.Issuer,
            audience: jwtSettings.Audience,
            claims: claims,
            expires: TimeHelper.ConvertToUtcPlus7(DateTime.Now.AddHours(jwtSettings.AccessTokenExpirationMinutes))
                .DateTime,
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public static string GetUserIdFromHttpContext(HttpContext httpContext)
    {
        try
        {
            if (!httpContext.Request.Headers.ContainsKey("Authorization"))
            {
                throw new UnauthorizedException("Need Authorization");
            }

            string? authorizationHeader = httpContext.Request.Headers["Authorization"];

            if (string.IsNullOrWhiteSpace(authorizationHeader) ||
                !authorizationHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                throw new UnauthorizedException($"Invalid authorization header: {authorizationHeader}");
            }

            string jwtToken = authorizationHeader["Bearer ".Length..].Trim();

            var tokenHandler = new JwtSecurityTokenHandler();

            if (!tokenHandler.CanReadToken(jwtToken))
            {
                throw new UnauthorizedException("Invalid token format");
            }

            JwtSecurityToken token = tokenHandler.ReadJwtToken(jwtToken);
            var idClaim = token.Claims.FirstOrDefault(claim => claim.Type == "Id");

            return idClaim?.Value ?? throw new UnauthorizedException("Cannot get userId from token");
        }
        catch (UnauthorizedException ex)
        {
            var errorResponse = new
            {
                data = "An unexpected error occurred.",
                message = ex.Message,
                statusCode = StatusCodes.Status401Unauthorized,
                code = "Unauthorized!"
            };

            var jsonResponse = System.Text.Json.JsonSerializer.Serialize(errorResponse);

            httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
            httpContext.Response.ContentType = "application/json";
            httpContext.Response.WriteAsync(jsonResponse).Wait();

            throw; // Re-throw the exception to maintain the error flow
        }
    }
}

[Serializable]
internal class UnauthorizedException : Exception
{
    public UnauthorizedException()
    {
    }

    public UnauthorizedException(string? message) : base(message)
    {
    }

    public UnauthorizedException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}