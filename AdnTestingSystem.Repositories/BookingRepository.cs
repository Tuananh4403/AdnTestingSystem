using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdnTestingSystem.Repositories.Data;
using AdnTestingSystem.Repositories.Models;
using Microsoft.EntityFrameworkCore;

namespace AdnTestingSystem.Repositories.AdnTestingSystem.Repositories.Implementations
{
    public class BookingRepository
    {
        private readonly AdnTestingDbContext _context;

        public BookingRepository(AdnTestingDbContext context)
        {
            _context = context;
        }

        public async Task<(List<Booking> Items, int TotalCount)> GetBookingListForStaffAsync(
       int page,
       int pageSize,
       string? searchTerm,
       BookingStatus? status)
        {
            var query = _context.Bookings
                .Include(b => b.Customer)
                    .ThenInclude(c => c.Profile)
                .Include(b => b.DnaTestService)
                    .ThenInclude(s => s.Prices) // Thêm include Prices để lấy giá service
                .Include(b => b.Transaction)
                .Include(b => b.Samples)
                .Include(b => b.TestResult)
                .Include(b => b.Rating)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var search = searchTerm.Trim().ToLower();
                query = query.Where(b =>
                    b.Id.ToString().Contains(search) ||
                    $"BK{b.Id:D6}".ToLower().Contains(search) ||
                    (b.Customer.Profile != null &&
                     b.Customer.Profile.FullName.ToLower().Contains(search)) ||
                    b.Customer.Email.ToLower().Contains(search)
                );
            }

            if (status.HasValue)
            {
                query = query.Where(b => b.Status == status.Value);
            }

            var totalCount = await query.CountAsync();

            // FIXED: Sort theo yêu cầu đúng
            // 1. Chưa duyệt (Status == Pending) trước, đã duyệt sau
            // 2. Theo ngày tạo mới nhất (BookingDate descending)
            query = query.OrderBy(b => b.Status == BookingStatus.Pending ? 0 : 1)
                         .ThenByDescending(b => b.BookingDate);

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }
        public class CreateBookingRequest
        {
            public int CustomerId { get; set; }
            public int ServiceId { get; set; }
            public SampleMethod SampleMethod { get; set; }
        }

        public class BookingListRequest
        {
            public int Page { get; set; } = 1;
            public int PageSize { get; set; } = 20;
            public string? SearchTerm { get; set; }
            public BookingStatus? Status { get; set; }
        }

        public class BookingListResponse
        {
            public List<BookingItemDto> Items { get; set; } = new();
            public int TotalItems { get; set; }
            public int TotalPages { get; set; }
            public int CurrentPage { get; set; }
            public int PageSize { get; set; }
        }

        public class BookingItemDto
        {
            public int Id { get; set; }
            public string BookingId { get; set; } = string.Empty;
            public string CustomerName { get; set; } = string.Empty;
            public string CustomerEmail { get; set; } = string.Empty;
            public string CustomerPhone { get; set; } = string.Empty;
            public string ServiceName { get; set; } = string.Empty;
            public BookingStatus Status { get; set; }
            public string StatusDisplay { get; set; } = string.Empty;
            public DateTime BookingDate { get; set; }
            public SampleMethod SampleMethod { get; set; }
            public string SampleMethodDisplay { get; set; } = string.Empty;
            public bool IsApproved { get; set; }
            public decimal? ServicePrice { get; set; }
            public PaymentStatus? PaymentStatus { get; set; }
            public string? PaymentStatusDisplay { get; set; }
            public decimal? TransactionAmount { get; set; }
            public DateTime? TransactionCreatedAt { get; set; }
            public int SampleCount { get; set; }
            public bool HasTestResult { get; set; }
            public bool HasRating { get; set; }
            public int? RatingStars { get; set; }
        }
    }
}
