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
using Microsoft.AspNetCore.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Security.Cryptography;

namespace AdnTestingSystem.Services.Services
{
    public class BookingService : IBookingService
    {
        private readonly IUnitOfWork _uow;
        private readonly IEmailSender _email;
        private readonly AdnTestingDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public BookingService(IUnitOfWork uow, IEmailSender email, AdnTestingDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _uow = uow;
            _email = email;
            _context = context;
            _httpContextAccessor = httpContextAccessor;
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
                .Include(b => b.Customer)
                .Include(b => b.BookingAttachments)
                .FirstOrDefaultAsync(b => b.Id == request.BookingId);

            if (booking == null)
            {
                return CommonResponse<string>.Fail("Booking not found");
            }

            if (request.Status.HasValue)
            {
                if (!Enum.IsDefined(typeof(BookingStatus), request.Status.Value))
                {
                    return CommonResponse<string>.Fail("Invalid status");
                }

                booking.Status = (BookingStatus)request.Status.Value;
            }

            if (!string.IsNullOrWhiteSpace(request.Note))
            {
                booking.Note = request.Note;
            }

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

            if (request.SampleCollectorId.HasValue)
            {
                var collectorExists = await _context.Users.AnyAsync(u => u.Id == request.SampleCollectorId.Value);
                if (!collectorExists)
                {
                    return CommonResponse<string>.Fail("Người thu mẫu không tồn tại");
                }

                booking.SampleCollectorId = request.SampleCollectorId.Value;
            }


            await _context.SaveChangesAsync();

            if (!string.IsNullOrWhiteSpace(booking.Customer?.Email))
            {
                var bookingCode = "ĐH" + booking.Id;

                var emailBody = new StringBuilder();
                emailBody.AppendLine("<p>Kính chào quý khách,</p>");
                emailBody.AppendLine($"<p>MATAP xin thông báo đơn hàng của bạn với mã <strong>{bookingCode}</strong> đã được cập nhật.</p>");
                emailBody.AppendLine("<p>Vui lòng kiểm tra đơn hàng của bạn trên phần mềm!</p>");
                emailBody.AppendLine("<p>Trân trọng,<br/>Admin</p>");
                await _email.SendAsync(
                    booking.Customer.Email,
                    "Cập nhật thông tin đơn hàng",
                    emailBody.ToString()
                );
            }

            return CommonResponse<string>.Ok("OK", "Cập nhật đơn hàng thành công!");
        }
        public async Task<CommonResponse<PagedResult<BookingListResponse>>> GetUserBookingsAsync(int userId, BookingListRequest request)
        {
            request.PageSize = request.PageSize <= 0 ? 20 : request.PageSize;
            request.Page = request.Page <= 0 ? 1 : request.Page;

            var query = _uow.Bookings.Query()
            .Include(b => b.DnaTestService)
            .Include(b => b.Transaction)
            .Include(b => b.Customer).ThenInclude(u => u.Profile)
            .Include(b => b.SampleCollector).ThenInclude(u => u.Profile)
            .Where(b => b.DeletedAt == null);

            if (!request.IsAll)
            {
                query = query.Where(b => b.CustomerId == userId);
            }

            if (request.IsAll && request.BookingId.HasValue)
            {
                query = query.Where(b => b.Id == request.BookingId.Value);
            }

            if (request.Status.HasValue)
                query = query.Where(b => b.Status == request.Status); 

            if (request.IsAll && request.IsAppointment)
            {
                query = query.Where(b => (int)b.Status >= 2);
            }

            if (request.IsAll && request.IsSampleReceipt)
            {
                query = query.Where(b => (int)b.Status >= 4 && b.IsSampleReceiptCreated == false);
            }

            if (request.IsAll && request.IsTestResult)
            {
                query = query.Where(b =>
                    (int)b.Status >= 5 &&
                    b.IsTestResultCreated == false &&
                    b.IsSampleReceiptCreated == true &&
                    _context.SampleReceipts.Any(r => r.BookingId == b.Id && r.Status == SampleReceipt.SampleReceiptStatus.Confirmed));
            }

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
                IsCivil = b.IsCivil,
                Note = b.Note,

                CustomerFullName = b.Customer.Profile?.FullName ?? "N/A",
                ApprovedAt = b.ApprovedAt?.ToString("dd-MM-yyyy HH:mm") ?? null,
                StatusTransaction = b.Transaction != null ? (int)b.Transaction.Status : -1,
                AppointmentTime = b.AppointmentTime?.ToString("dd-MM-yyyy HH:mm"),
                SampleCollector = b.SampleCollector?.Profile?.FullName,

                CustomerEmail = b.Customer?.Email,
                CustomerPhone = b.Customer?.Profile?.Phone,
                CustomerAddress = b.Customer?.Profile?.Address,
                CustomerGender = b.Customer?.Profile?.Gender,
                CustomerDob = b.Customer?.Profile?.DateOfBirth?.ToString("dd-MM-yyyy")
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
        public async Task<CommonResponse<string>> UpdateBookingCustomerAsync(int userId, int bookingId, UpdateBookingCustomerRequest request)
        {
            var booking = await _uow.Bookings.GetAsync(b => b.Id == bookingId && b.CustomerId == userId);
            if (booking == null)
                return CommonResponse<string>.Fail("Không tìm thấy đơn hàng cần cập nhật.");

            var service = await _uow.DnaTestServices.GetByIdAsync(request.DnaTestServiceId);
            if (service == null || !service.IsActive)
                return CommonResponse<string>.Fail("Dịch vụ không hợp lệ.");

            DateTime? appointmentTime = null;
            if (!string.IsNullOrWhiteSpace(request.AppointmentDate))
            {
                if (!DateTime.TryParseExact(request.AppointmentDate, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedDate))
                    return CommonResponse<string>.Fail("Ngày thu mẫu không hợp lệ.");
                appointmentTime = parsedDate;
            }

            booking.DnaTestServiceId = request.DnaTestServiceId;
            booking.IsCivil = request.IsCivil;
            booking.SampleMethod = (SampleMethod)request.SampleMethod;
            booking.ResultTimeType = (ResultTimeType)request.ResultTimeType;
            booking.AppointmentTime = appointmentTime;
            booking.TotalPrice = request.TotalPrice;
            booking.UpdatedAt = DateTime.UtcNow;
            booking.UpdatedBy = userId;

            await _uow.CompleteAsync();

            return CommonResponse<string>.Ok(string.Empty,"Cập nhật đơn hàng thành công!");
        }
        public async Task<CommonResponse<string>> SoftDeleteBookingAsync(int userId, int bookingId)
        {
            var booking = await _uow.Bookings.GetAsync(b => b.Id == bookingId && b.DeletedAt == null);
            if (booking == null)
                return CommonResponse<string>.Fail("Không tìm thấy đơn hàng!");

            booking.DeletedAt = DateTime.UtcNow;
            booking.DeletedBy = userId;
            booking.UpdatedAt = DateTime.UtcNow;
            booking.UpdatedBy = userId;

            await _uow.CompleteAsync();

            return CommonResponse<string>.Ok(string.Empty, "Xóa đơn hàng thành công!");
        }

        public async Task<CommonResponse<string>> GenerateVnPayPaymentUrlAsync(int bookingId, int userId)
        {
            var booking = await _uow.Bookings.GetAsync(b => b.Id == bookingId && b.CustomerId == userId);
            var now = DateTime.UtcNow;
            string GenerateTransactionCode() => $"TXN-{now:yyyyMMddHHmmssfff}-{bookingId}";

            var httpContext = _httpContextAccessor.HttpContext;
            var query = httpContext?.Request.Query;

            string paymentMethod = query?.ContainsKey("method") == true ? query["method"].ToString() : "UNKNOWN";
            string transactionCode = GenerateTransactionCode();

            if (booking == null)
            {
                await _uow.Transactions.AddAsync(new Transaction
                {
                    BookingId = bookingId,
                    CreatedBy = userId,
                    Amount = 0,
                    PaymentMethod = paymentMethod,
                    TransactionCode = transactionCode,
                    Status = PaymentStatus.Failed,
                    Message = "Thanh toán thất bại: Không tìm thấy đơn hàng.",
                    CreatedAt = now
                });
                await _uow.CompleteAsync();
                return CommonResponse<string>.Fail("Thanh toán thất bại! Vui lòng kiểm tra Lịch sử giao dịch để biết thêm thông tin chi tiết.");
            }

            if (booking.Status != BookingStatus.Pending)
            {
                await _uow.Transactions.AddAsync(new Transaction
                {
                    BookingId = bookingId,
                    CreatedBy = userId,
                    Amount = 0,
                    PaymentMethod = paymentMethod,
                    TransactionCode = transactionCode,
                    Status = PaymentStatus.Failed,
                    Message = "Thanh toán thất bại: Đơn hàng không ở trạng thái chờ thanh toán.",
                    CreatedAt = now
                });
                await _uow.CompleteAsync();
                return CommonResponse<string>.Fail("Thanh toán thất bại! Vui lòng kiểm tra Lịch sử giao dịch để biết thêm thông tin chi tiết.");
            }

            if (query == null || !query.ContainsKey("amount") || !int.TryParse(query["amount"], out var userAmount))
            {
                await _uow.Transactions.AddAsync(new Transaction
                {
                    BookingId = bookingId,
                    CreatedBy = userId,
                    Amount = 0,
                    PaymentMethod = paymentMethod,
                    TransactionCode = transactionCode,
                    Status = PaymentStatus.Failed,
                    Message = "Thanh toán thất bại: Số tiền thanh toán không hợp lệ.",
                    CreatedAt = now
                });
                await _uow.CompleteAsync();
                return CommonResponse<string>.Fail("Thanh toán thất bại! Vui lòng kiểm tra Lịch sử giao dịch để biết thêm thông tin chi tiết.");
            }

            if (userAmount != booking.TotalPrice)
            {
                string reason = userAmount < booking.TotalPrice
                    ? "Số tiền thanh toán nhỏ hơn tổng tiền dịch vụ."
                    : "Số tiền thanh toán lớn hơn tổng tiền dịch vụ.";

                await _uow.Transactions.AddAsync(new Transaction
                {
                    BookingId = bookingId,
                    CreatedBy = userId,
                    Amount = userAmount,
                    PaymentMethod = paymentMethod,
                    TransactionCode = transactionCode,
                    Status = PaymentStatus.Failed,
                    Message = $"Thanh toán thất bại: {reason}",
                    CreatedAt = now
                });
                await _uow.CompleteAsync();
                return CommonResponse<string>.Fail("Thanh toán thất bại! Vui lòng kiểm tra Lịch sử giao dịch để biết thêm thông tin chi tiết.");
            }

            booking.Status = BookingStatus.Paid;
            booking.UpdatedAt = now;
            booking.UpdatedBy = userId;
            _uow.Bookings.Update(booking);

            await _uow.Transactions.AddAsync(new Transaction
            {
                BookingId = bookingId,
                CreatedBy = userId,
                Amount = userAmount,
                PaymentMethod = paymentMethod,
                TransactionCode = transactionCode,
                Status = PaymentStatus.Paid,
                Message = $"Thanh toán đơn hàng ĐH{bookingId} thành công.",
                CreatedAt = now
            });

            await _uow.CompleteAsync();
            return CommonResponse<string>.Ok(string.Empty, "Thanh toán thành công! Vui lòng kiểm tra Lịch sử giao dịch để biết thêm thông tin chi tiết.");
        }

        public async Task<CommonResponse<BookingStatusStatisticDto>> GetBookingStatisticsAsync(int? month = null)
        {
            var targetMonth = month ?? DateTime.UtcNow.Month;
            var targetYear = DateTime.UtcNow.Year;

            var bookings = await _uow.Bookings.Query()
                .Where(b => b.BookingDate.Month == targetMonth && b.BookingDate.Year == targetYear)
                .ToListAsync();

            int total = bookings.Count;
            if (total == 0)
            {
                return CommonResponse<BookingStatusStatisticDto>.Ok(new BookingStatusStatisticDto
                {
                    TotalBookings = 0,
                    CompletedCount = 0,
                    CancelledCount = 0,
                    ProcessingCount = 0,
                    CompletedPercentage = 0,
                    CancelledPercentage = 0,
                    ProcessingPercentage = 0
                }, $"Không có booking nào trong tháng {targetMonth}.");
            }

            int completed = bookings.Count(b => b.Status == BookingStatus.Completed);
            int cancelled = bookings.Count(b => b.Status == BookingStatus.Cancelled);
            int processing = total - completed - cancelled;

            var result = new BookingStatusStatisticDto
            {
                TotalBookings = total,
                CompletedCount = completed,
                CompletedPercentage = Math.Round((double)completed * 100 / total, 2),

                CancelledCount = cancelled,
                CancelledPercentage = Math.Round((double)cancelled * 100 / total, 2),

                ProcessingCount = processing,
                ProcessingPercentage = Math.Round((double)processing * 100 / total, 2)
            };

            return CommonResponse<BookingStatusStatisticDto>.Ok(result);
        }
    }
}
