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
    public interface IUserManagementService
    {
        Task<CommonResponse<string>> CreateUserAsync(CreateUserRequest request);
        Task<CommonResponse<PagedResult<UserResponse>>> GetAllUsersAsync(int page, int pageSize);
        Task<CommonResponse<List<UserSimpleResponse>>> GetStaffListAsync(string? search);

    }
}
