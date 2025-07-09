using AdnTestingSystem.Repositories.Data;
using AdnTestingSystem.Repositories.Models;
using AdnTestingSystem.Repositories.Repositories.Repository;
using AdnTestingSystem.Repositories.UnitOfWork;
using AdnTestingSystem.Services.Helpers;
using AdnTestingSystem.Services.Interfaces;
using AdnTestingSystem.Services.Requests;
using AdnTestingSystem.Services.Responses;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Text;

namespace AdnTestingSystem.Services.Services
{
    public class BookingService : IBookingService
    {
        private readonly IUnitOfWork _uow;
        private readonly IEmailSender _email;
        private readonly AdnTestingDbContext _context;
        public BookingService(IUnitOfWork uow, IEmailSender email, AdnTestingDbContext context)
        {
            _uow = uow;
            _email = email;
            _context = context;
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
            if (user == null)
                return CommonResponse<string>.Fail("Không tìm thấy thông tin người dùng.");

            var service = await _uow.DnaTestServices.GetByIdAsync(req.DnaTestServiceId);
            if (service == null || !service.IsActive)
                return CommonResponse<string>.Fail("Dịch vụ không hợp lệ.");

            DateTime? appointmentTime = null;
            if (!string.IsNullOrWhiteSpace(req.AppointmentDate))
            {
                if (!DateTime.TryParseExact(req.AppointmentDate, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedDate))
                {
                    return CommonResponse<string>.Fail("Ngày thu mẫu không hợp lệ. Định dạng đúng: dd-MM-yyyy");
                }

                appointmentTime = parsedDate;
            }

            var booking = new Booking
            {
                CustomerId = userId,
                CreatedBy = userId,
                UpdatedBy = userId,
                DnaTestServiceId = req.DnaTestServiceId,
                SampleMethod = (SampleMethod)req.SampleMethod,
                ResultTimeType = (ResultTimeType)req.ResultTimeType,
                IsCivil = req.IsCivil,
                BookingDate = DateTime.UtcNow,
                AppointmentTime = appointmentTime,
                TotalPrice = req.TotalPrice,
                Status = BookingStatus.Pending,
            };

            await _uow.Bookings.AddAsync(booking);
            await _uow.CompleteAsync();

            var bookingCode = "ĐH" + booking.Id; 

            var sampleMethodLabel = Enum.GetName(typeof(SampleMethod), req.SampleMethod);
            var resultTimeLabel = Enum.GetName(typeof(ResultTimeType), req.ResultTimeType);
            var statusLabel = BookingStatus.Pending.ToString();

            var bodyBuilder = new StringBuilder();
            bodyBuilder.AppendLine("<p>Kính chào quý khách,</p>");
            bodyBuilder.AppendLine("<p>Cảm ơn quý khách đã tin tưởng và đặt dịch vụ xét nghiệm tại hệ thống của chúng tôi.</p>");
            bodyBuilder.AppendLine("<p><strong>Thông tin đơn hàng của quý khách như sau:</strong></p>");

            bodyBuilder.AppendLine("<table cellpadding='5' cellspacing='0' style='border-collapse:collapse;'>");
            bodyBuilder.AppendLine($"<tr><td><b>Mã đơn hàng:</b></td><td>{bookingCode}</td></tr>");
            bodyBuilder.AppendLine($"<tr><td><b>Loại xét nghiệm:</b></td><td>{(booking.IsCivil ? "Dân sự" : "Hành chính")}</td></tr>");
            bodyBuilder.AppendLine($"<tr><td><b>Loại dịch vụ:</b></td><td>{service.Name}</td></tr>");
            bodyBuilder.AppendLine($"<tr><td><b>Phương thức lấy mẫu:</b></td><td>{EnumHelpers.GetSampleMethodLabel(booking.SampleMethod)}</td></tr>");
            bodyBuilder.AppendLine($"<tr><td><b>Thời gian nhận kết quả:</b></td><td>{EnumHelpers.GetResultTimeLabel(booking.ResultTimeType)}</td></tr>");
            bodyBuilder.AppendLine($"<tr><td><b>Tổng tiền:</b></td><td>{booking.TotalPrice:N0}đ</td></tr>");
            bodyBuilder.AppendLine($"<tr><td><b>Ngày thu mẫu:</b></td><td>{booking.AppointmentTime?.ToString("dd-MM-yyyy") ?? "-"}</td></tr>");
            bodyBuilder.AppendLine($"<tr><td><b>Ngày đặt hàng:</b></td><td>{booking.BookingDate.ToString("dd-MM-yyyy")}</td></tr>");
            bodyBuilder.AppendLine($"<tr><td><b>Trạng thái đơn hàng:</b></td><td>{EnumHelpers.GetStatusLabel(booking.Status)}</td></tr>");
            bodyBuilder.AppendLine("</table>");

            bodyBuilder.AppendLine("<p>Vui lòng tiến hành thanh toán để chúng tôi có thể xử lý đơn hàng sớm nhất cho quý khách.</p>");
            bodyBuilder.AppendLine("<p>Trân trọng cảm ơn quý khách!</p>");

            await _email.SendAsync(user.Email, "Xác nhận đặt dịch vụ xét nghiệm", bodyBuilder.ToString());

            return CommonResponse<string>.Ok(string.Empty, "Đặt dịch vụ thành công! Vui lòng kiểm tra email để biết thêm thông tin chi tiết và tiến hành thanh toán.");
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

        public async Task<CommonResponse<string>> UpdateBookingAsync(int staffId, UpdateBookingRequest request)
        {
            var booking = await _context.Bookings
                .Include(b => b.BookingAttachments)
                .FirstOrDefaultAsync(b => b.Id == request.BookingId);

            if (booking == null)
            {
                return CommonResponse<string>.Fail("Booking not found");
            }

            if (!Enum.TryParse<BookingStatus>(request.Status, true, out var status))
            {
                return CommonResponse<string>.Fail("Invalid status");
            }

            booking.Status = status;
            booking.Note = request.Note;
            booking.UpdatedAt = DateTime.UtcNow;
            booking.UpdatedBy = staffId;

            if (!string.IsNullOrEmpty(request.AttachmentImageUrl))
            {
                booking.BookingAttachments.Add(new BookingAttachment
                {
                    FileUrl = request.AttachmentImageUrl,
                    UploadedAt = DateTime.UtcNow,
                    UploadedBy = staffId
                });

            }

            await _context.SaveChangesAsync();

            return CommonResponse<string>.Ok("OK", "Booking updated successfully");
        }
        public async Task<CommonResponse<PagedResult<BookingListResponse>>> GetUserBookingsAsync(int userId, BookingListRequest request)
        {
            request.PageSize = request.PageSize <= 0 ? 20 : request.PageSize;
            request.Page = request.Page <= 0 ? 1 : request.Page;

            var query = _uow.Bookings.Query()
                .Include(b => b.DnaTestService)
                .Where(b => b.CustomerId == userId);

            if (request.Status.HasValue)
                query = query.Where(b => b.Status == request.Status);

            if (!string.IsNullOrEmpty(request.SortBy) && request.SortBy.Equals("status", StringComparison.OrdinalIgnoreCase))
            {
                query = request.SortDesc
                    ? query.OrderByDescending(b => b.Status)
                    : query.OrderBy(b => b.Status);
            }
            else
            {
                query = query.OrderByDescending(b => b.Status)
                             .ThenByDescending(b => b.BookingDate);
            }

            var paged = await PaginationHelper.ToPagedResultAsync(query, request.Page, request.PageSize);

            var resultItems = paged.Items.Select(b => new BookingListResponse
            {
                Id = b.Id,
                ServiceName = b.DnaTestService.Name,
                SampleMethod = (int)b.SampleMethod,
                ResultTime = (int)b.ResultTimeType,
                TotalPrice = b.TotalPrice,
                SampleDate = b.AppointmentTime?.ToString("dd-MM-yyyy"),
                BookingDate = b.BookingDate.ToString("dd-MM-yyyy"),
                Status = (int)b.Status,
                IsCivil = b.IsCivil
            }).ToList();

            return CommonResponse<PagedResult<BookingListResponse>>.Ok(new PagedResult<BookingListResponse>
            {
                Items = resultItems,
                TotalItems = paged.TotalItems,
                Page = paged.Page,
                PageSize = paged.PageSize
            });
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
