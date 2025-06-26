using AdnTestingSystem.Repositories.Models;
using AdnTestingSystem.Repositories.Repositories.Repository;
using AdnTestingSystem.Repositories.UnitOfWork;
using AdnTestingSystem.Services.Interfaces;
using AdnTestingSystem.Services.Requests;
using AdnTestingSystem.Services.Responses;
using Microsoft.EntityFrameworkCore;

namespace AdnTestingSystem.Services.Services
{
    public class BookingService : IBookingService
    {
        private readonly IUnitOfWork _uow;
        private readonly IEmailSender _email;

        public BookingService(IUnitOfWork uow, IEmailSender email)
        {
            _uow = uow;
            _email = email;
        }

        public async Task<CommonResponse<IEnumerable<DnaTestService>>> GetServicesAsync(bool isCivil)
        {
            var services = await _uow.DnaTestServices.FindAsync(s => s.IsCivil == isCivil && s.IsActive);
            return CommonResponse<IEnumerable<DnaTestService>>.Ok(services);
        }

        public async Task<CommonResponse<decimal>> GetServicePriceAsync(int serviceId, ResultTimeType resultType, SampleMethod sampleMethod)
        {
            var rule = await _uow.ServicePrices.GetAsync(p =>
                p.DnaTestServiceId == serviceId &&
                p.ResultTimeType == resultType &&
                p.SampleMethod == sampleMethod &&
                p.AppliedFrom <= DateTime.UtcNow,
                orderBy: q => q.OrderByDescending(x => x.AppliedFrom));

            return rule == null
                ? CommonResponse<decimal>.Fail("Không tìm thấy mức giá phù hợp.")
                : CommonResponse<decimal>.Ok(rule.Price);
        }

        public async Task<CommonResponse<string>> CreateBookingAsync(int userId, CreateBookingRequest req)
        {
            var user = await _uow.Users.GetAsync(u => u.Id == userId);
            if (user == null || user.Profile == null)
                return CommonResponse<string>.Fail("Không tìm thấy thông tin người dùng.");

            var service = await _uow.DnaTestServices.GetByIdAsync(req.DnaTestServiceId);
            if (service == null || !service.IsActive)
                return CommonResponse<string>.Fail("Dịch vụ không hợp lệ.");

            var priceRule = await _uow.ServicePrices.GetAsync(p =>
                p.DnaTestServiceId == req.DnaTestServiceId &&
                p.ResultTimeType == req.ResultTimeType &&
                p.SampleMethod == req.SampleMethod &&
                p.IsCivil == service.IsCivil &&
                p.AppliedFrom <= DateTime.UtcNow,
                orderBy: q => q.OrderByDescending(x => x.AppliedFrom));

            if (priceRule == null)
                return CommonResponse<string>.Fail("Không tìm thấy giá phù hợp.");

            var booking = new Booking
            {
                CustomerId = userId,
                DnaTestServiceId = req.DnaTestServiceId,
                BookingDate = DateTime.UtcNow,
                ResultTimeType = req.ResultTimeType,
                SampleMethod = req.SampleMethod,
                AppointmentTime = req.SampleMethod == SampleMethod.SelfAtHome ? null : req.AppointmentTime,
                Status = BookingStatus.Pending,
                TotalPrice = priceRule.Price
            };

            await _uow.Bookings.AddAsync(booking);
            await _uow.CompleteAsync();

            await _email.SendAsync(user.Email, "Đơn đặt dịch vụ xét nghiệm",
                $"Bạn đã đặt dịch vụ xét nghiệm {service.Name}. Tổng tiền: {priceRule.Price}đ");

            return CommonResponse<string>.Ok("Đặt dịch vụ thành công. Vui lòng thanh toán.");
        }

        public async Task<CommonResponse<IEnumerable<Booking>>> GetBookingHistoryAsync(int userId)
        {
            var bookings = await _uow.Bookings.FindAsync(b => b.CustomerId == userId);
            return CommonResponse<IEnumerable<Booking>>.Ok(bookings);
        }

        public async Task<CommonResponse<string>> PayBookingAsync(int bookingId, int userId)
        {
            var booking = await _uow.Bookings.GetAsync(b => b.Id == bookingId && b.CustomerId == userId);
            if (booking == null) return CommonResponse<string>.Fail("Không tìm thấy đơn hàng.");

            if (booking.Status != BookingStatus.Pending)
                return CommonResponse<string>.Fail("Đơn hàng đã thanh toán hoặc không hợp lệ.");

            booking.Status = BookingStatus.Paid;

            var transaction = new Transaction
            {
                BookingId = bookingId,
                PaymentMethod = "Sandbox",
                TransactionCode = $"TXN-{Guid.NewGuid().ToString()[..8]}",
                Status = PaymentStatus.Paid,
                Amount = booking.TotalPrice,
                CreatedAt = DateTime.UtcNow
            };

            await _uow.Transactions.AddAsync(transaction);
            await _uow.CompleteAsync();

            return CommonResponse<string>.Ok("Thanh toán thành công.");
        }


        public async Task<BookingListResponse<BookingStaffDto>> GetBookingListForStaffAsync(BookingListRequest request)
        {
            // Fix page size
            if (request.PageSize <= 0 || request.PageSize > 100)
                request.PageSize = 20;

            if (request.Page <= 0)
                request.Page = 1;

            var query = _uow.Bookings.Query()
                .Include(b => b.Customer).ThenInclude(c => c.Profile)
                .Include(b => b.DnaTestService).ThenInclude(s => s.Prices)
                .Include(b => b.Transaction)
                .Include(b => b.Samples)
                .Include(b => b.TestResult)
                .Include(b => b.Rating)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var search = request.SearchTerm.Trim().ToLower();

                if (int.TryParse(search, out int bookingId))
                {
                    query = query.Where(b => b.Id == bookingId);
                }
                else
                {
                    query = query.Where(b =>
                        (b.Customer.Profile != null && b.Customer.Profile.FullName.ToLower().Contains(search)) ||
                        b.Status.ToString().ToLower().Contains(search)
                    );
                }
            }
            if (request.Status.HasValue)
            {
                query = query.Where(b => b.Status == request.Status.Value);
            }

            int totalCount = await query.CountAsync();

            query = query
                .OrderBy(b => b.ApprovedAt.HasValue)         
                .ThenByDescending(b => b.BookingDate);

            var items = await query
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(b => new BookingStaffDto
                {
                    Id = b.Id,
                    CustomerName = b.Customer.Profile.FullName,
                    CustomerEmail = b.Customer.Email,
                    Status = b.Status,
                    BookingDate = b.BookingDate,
                    ServiceName = b.DnaTestService.Name,
                    TotalPrice = b.TotalPrice
                })
                .ToListAsync();

            return new BookingListResponse<BookingStaffDto>
            {
                Items = items,
                TotalItems = totalCount,
                TotalPages = (int)Math.Ceiling((double)totalCount / request.PageSize),
                CurrentPage = request.Page,
                PageSize = request.PageSize
            };

        }

        public async Task<bool> ApproveBookingAsync(int bookingId, int approverUserId)
        {
            var booking = await _uow.Bookings.GetAsync(b => b.Id == bookingId);

            if (booking == null || booking.ApprovedAt != null)
                return false;

            var now = DateTime.UtcNow;

            booking.ApprovedBy = approverUserId;
            booking.ApprovedAt = now;
            booking.UpdatedAt = now;
            booking.UpdatedBy = approverUserId;

            await _uow.CompleteAsync();
            return true;
        }

    }
}
