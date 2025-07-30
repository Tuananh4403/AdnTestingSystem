using AdnTestingSystem.Repositories.Models;
using AdnTestingSystem.Repositories.Repositories.Repository;
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
        Task<CommonResponse<PagedResult<BookingListResponse>>> GetUserBookingsAsync(int userId, BookingListRequest request);
        Task<bool> ApproveBookingAsync(int bookingId, int approvedByUserId);
        Task<CommonResponse<string>> UpdateBookingCustomerAsync(int userId, int bookingId, UpdateBookingCustomerRequest request);
        Task<CommonResponse<string>> SoftDeleteBookingAsync(int userId, int bookingId);
        Task<CommonResponse<string>> GenerateVnPayPaymentUrlAsync(int bookingId, int userId);
        Task<CommonResponse<BookingStatusStatisticDto>> GetBookingStatisticsAsync(int? month = null);
    }

}
