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
    public interface IBookingService
    {
        Task<CommonResponse<IEnumerable<DnaTestService>>> GetServicesAsync(bool isCivil);
        Task<CommonResponse<decimal>> GetServicePriceAsync(int serviceId, ResultTimeType resultType, SampleMethod sampleMethod);
        Task<CommonResponse<string>> CreateBookingAsync(int userId, CreateBookingRequest request);
        Task<CommonResponse<IEnumerable<Booking>>> GetBookingHistoryAsync(int userId);
        Task<CommonResponse<string>> PayBookingAsync(int bookingId, int userId);
        Task<CommonResponse<string>> UpdateBookingAsync(int staffId, UpdateBookingRequest request);
        Task<CommonResponse<BookingListResponse<BookingStaffDto>>> GetBookingListForStaffAsync(BookingListRequest request);
        Task<CommonResponse<string>> ApproveBookingAsync(int bookingId, int approvedByUserId);

    }

}
