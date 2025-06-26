using AdnTestingSystem.Repositories.Data;
using AdnTestingSystem.Repositories.Models;
using AdnTestingSystem.Repositories.Repositories.Repository;
using AdnTestingSystem.Repositories.UnitOfWork;
using AdnTestingSystem.Services.Interfaces;
using AdnTestingSystem.Services.Requests;
using AdnTestingSystem.Services.Responses;
using Microsoft.EntityFrameworkCore;

namespace AdnTestingSystem.Services.Services
{
    public class SampleService : ISampleService
    {
        private readonly IUnitOfWork _unitOfWork;
        public SampleService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<CommonResponse<PagedResult<AtHomeSampleBookingResponse>>> GetAtHomeSampleBooking(BookingListRequest request)
        {
            var query = _unitOfWork.Bookings
                .Query()
                .Where(b => b.SampleMethod == SampleMethod.SelfAtHome)
                .Include(b => b.Customer)
                    .ThenInclude(u => u.Profile)
                .OrderByDescending(b => b.CreatedAt);

            var data = await query
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            var pagedItems = data.Select(b => new AtHomeSampleBookingResponse
            {
                BookingId = b.Id,
                SampleMethod = b.SampleMethod.ToString(),
                AppointmentTime = b.AppointmentTime,
                FullName = b.Customer.Profile.FullName,
                Phone = b.Customer.Profile.Phone,
                Email = b.Customer.Email
            }).ToList();

            var result = new PagedResult<AtHomeSampleBookingResponse>
            {
                Items = pagedItems,
                TotalItems = pagedItems.Count,
                Page = request.Page,
                PageSize = request.PageSize
            };

            return CommonResponse<PagedResult<AtHomeSampleBookingResponse>>.Ok(result);
        }


    }
}
