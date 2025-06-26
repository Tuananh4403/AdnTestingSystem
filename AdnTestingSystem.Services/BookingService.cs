using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdnTestingSystem.Repositories.AdnTestingSystem.Repositories.Implementations;
using AdnTestingSystem.Repositories.Models;
using static AdnTestingSystem.Repositories.AdnTestingSystem.Repositories.Implementations.BookingRepository;

namespace AdnTestingSystem.Services
{
    public class BookingService
    {
        private readonly BookingRepository _bookingRepository;

        public BookingService(BookingRepository bookingRepository)
        {
            _bookingRepository = bookingRepository;
        }

        public async Task<BookingListResponse> GetBookingListForStaffAsync(BookingListRequest request)
        {
            // Validate and set defaults
            if (request.PageSize <= 0 || request.PageSize > 100)
            {
                request.PageSize = 20; // Default page size
            }

            if (request.Page <= 0)
            {
                request.Page = 1;
            }

            // Get data from repository
            var (items, totalCount) = await _bookingRepository.GetBookingListForStaffAsync(
                request.Page,
                request.PageSize,
                request.SearchTerm,
                request.Status);

            // Calculate pagination info
            var totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize);

            // Map entities to DTOs
            var bookingDtos = items.Select(MapToBookingItemDto).ToList();

            return new BookingListResponse
            {
                Items = bookingDtos,
                TotalItems = totalCount,
                TotalPages = totalPages,
                CurrentPage = request.Page,
                PageSize = request.PageSize
            };
        }

        private BookingItemDto MapToBookingItemDto(Booking booking)
        {
            return new BookingItemDto
            {
                Id = booking.Id,
                BookingId = $"BK{booking.Id:D6}", // Format: BK000001
                CustomerName = booking.Customer.Profile != null
                    ? booking.Customer.Profile.FullName.Trim()
                    : "N/A",
                CustomerEmail = booking.Customer.Email,
                CustomerPhone = booking.Customer.Profile?.Phone ?? "",
                ServiceName = booking.DnaTestService?.Name ?? "N/A",
                Status = booking.Status,
                StatusDisplay = GetStatusDisplay(booking.Status),
                BookingDate = booking.BookingDate,
                SampleMethod = booking.SampleMethod,
                SampleMethodDisplay = GetSampleMethodDisplay(booking.SampleMethod),
                IsApproved = booking.Status != BookingStatus.Pending, // Pending = chưa duyệt, khác Pending = đã duyệt
                ServicePrice = GetServicePrice(booking.DnaTestService),
                PaymentStatus = booking.Transaction?.Status,
                PaymentStatusDisplay = booking.Transaction != null ? GetPaymentStatusDisplay(booking.Transaction.Status) : null,
                TransactionAmount = booking.Transaction?.Amount,
                TransactionCreatedAt = booking.Transaction?.CreatedAt,
                SampleCount = booking.Samples?.Count ?? 0,
                HasTestResult = booking.TestResult != null,
                HasRating = booking.Rating != null,
                RatingStars = booking.Rating?.Stars
            };
        }

        private decimal? GetServicePrice(DnaTestService? service)
        {
            if (service?.Prices == null || !service.Prices.Any())
                return null;

            // Lấy giá hiện tại (có thể là giá mới nhất hoặc theo logic business khác)
            return service.Prices.OrderByDescending(p => p.Id).FirstOrDefault()?.Price;
        }

        private string GetStatusDisplay(BookingStatus status)
        {
            return status switch
            {
                BookingStatus.Pending => "Chờ thanh toán",
                BookingStatus.Paid => "Đã thanh toán",
                BookingStatus.KitSent => "Đã gửi kit",
                BookingStatus.SampleCollected => "Đã thu mẫu",
                BookingStatus.InLab => "Đang xét nghiệm",
                BookingStatus.Completed => "Hoàn thành",
                BookingStatus.Cancelled => "Đã hủy",
                _ => status.ToString()
            };
        }

        private string GetPaymentStatusDisplay(PaymentStatus status)
        {
            return status switch
            {
                PaymentStatus.Pending => "Chờ thanh toán",
                PaymentStatus.Paid => "Đã thanh toán",
                PaymentStatus.Failed => "Thanh toán thất bại",
                _ => status.ToString()
            };
        }

        private string GetSampleMethodDisplay(SampleMethod method)
        {
            return method switch
            {
                SampleMethod.SelfAtHome => "Tự thu tại nhà",
                SampleMethod.StaffAtHome => "Nhân viên thu tại nhà",
                SampleMethod.AtClinic => "Thu tại phòng khám",
                _ => method.ToString()
            };
        }
    }
}