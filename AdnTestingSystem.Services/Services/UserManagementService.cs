using AdnTestingSystem.Repositories.Models;
using AdnTestingSystem.Repositories.UnitOfWork;
using AdnTestingSystem.Services.Helpers;
using AdnTestingSystem.Services.Interfaces;
using AdnTestingSystem.Services.Requests;
using AdnTestingSystem.Services.Responses;
using Microsoft.EntityFrameworkCore;

namespace AdnTestingSystem.Services.Services
{
    public class UserManagementService : IUserManagementService
    {
        private readonly IUnitOfWork _uow;

        public UserManagementService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<CommonResponse<string>> CreateUserAsync(CreateUserRequest request)
        {
            var existing = await _uow.Users.GetAsync(u => u.Email == request.Email);
            if (existing != null)
                return CommonResponse<string>.Fail("Email đã được sử dụng.");

            var hash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            var user = new User
            {
                Email = request.Email,
                PasswordHash = hash,
                Role = request.Role,
                IsEmailConfirmed = true,
                Profile = new UserProfile
                {
                    FullName = request.FullName,
                    Phone = request.Phone,
                    Address = request.Address,
                    DateOfBirth = string.IsNullOrWhiteSpace(request.DateOfBirth)
                    ? null
                    : DateTime.ParseExact(request.DateOfBirth, "dd-MM-yyyy", null),
                    Gender = request.Gender
                }
            };

            await _uow.Users.AddAsync(user);
            await _uow.CompleteAsync();

            return CommonResponse<string>.Ok(string.Empty, "Tạo tài khoản mới thành công.");
        }
        public async Task<CommonResponse<PagedResult<UserResponse>>> GetAllUsersAsync(int page, int pageSize)
        {
            var query = _uow.Users
                .Query()
                .Include(u => u.Profile)
                .Where(u => u.IsEmailConfirmed);

            var paged = await PaginationHelper.ToPagedResultAsync(query, page, pageSize);

            var mappedUsers = paged.Items.Select(u => new UserResponse
            {
                Id = u.Id,
                Email = u.Email,
                Role = u.Role.ToString(),
                FullName = u.Profile?.FullName,
                Phone = u.Profile?.Phone,
                DateOfBirth = u.Profile?.DateOfBirth?.ToString("dd-MM-yyyy"),
                Gender = u.Profile?.Gender,
                Position = u.Profile?.Position
            }).ToList();

            var result = new PagedResult<UserResponse>
            {
                Items = mappedUsers,
                Page = paged.Page,
                PageSize = paged.PageSize,
                TotalItems = paged.TotalItems
            };

            return CommonResponse<PagedResult<UserResponse>>.Ok(result);
        }

        public async Task<CommonResponse<List<UserSimpleResponse>>> GetStaffListAsync(string? search)
        {
            var query = _uow.Users
                .Query()
                .Include(u => u.Profile)
                .Where(u => u.Role == UserRole.Staff && u.IsEmailConfirmed);

            if (!string.IsNullOrWhiteSpace(search))
            {
                var lowerSearch = search.ToLower();
                query = query.Where(u => u.Profile != null && u.Profile.FullName.ToLower().Contains(lowerSearch));
            }

            var staffList = await query
                .Select(u => new UserSimpleResponse
                {
                    Id = u.Id,
                    FullName = u.Profile!.FullName
                })
                .ToListAsync();

            return CommonResponse<List<UserSimpleResponse>>.Ok(staffList);
        }

    }
}
