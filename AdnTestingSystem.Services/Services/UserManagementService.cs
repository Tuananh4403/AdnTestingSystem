using AdnTestingSystem.Repositories.Models;
using AdnTestingSystem.Repositories.UnitOfWork;
using AdnTestingSystem.Services.Interfaces;
using AdnTestingSystem.Services.Requests;
using AdnTestingSystem.Services.Responses;

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
                    DateOfBirth = request.DateOfBirth
                }
            };

            await _uow.Users.AddAsync(user);
            await _uow.CompleteAsync();

            return CommonResponse<string>.Ok("Tạo người dùng thành công.");
        }
    }
}
