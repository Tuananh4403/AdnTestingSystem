using AdnTestingSystem.Services.Requests;
using AdnTestingSystem.Services.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdnTestingSystem.Services.Interfaces
{
    public interface IAuthService
    {
        Task<CommonResponse<string>> RegisterAsync(RegisterRequest request);
        Task<CommonResponse<string>> VerifyEmailAsync(VerifyEmailRequest request);
        Task<CommonResponse<string>> LoginAsync(LoginRequest request);
    }

}
