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
                .Where(b => b.CustomerId == userId && b.DeletedAt == null);

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
            if (booking == null)
                return CommonResponse<string>.Fail("Không tìm thấy đơn hàng.");

            if (booking.Status != BookingStatus.Pending)
                return CommonResponse<string>.Fail("Đơn hàng không hợp lệ để thanh toán.");

            const string vnp_ReturnUrl = "https://4c31d8836606.ngrok-free.app/vnpay-return";
            const string vnp_Url = "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html";
            const string vnp_TmnCode = "OYSX906U";
            const string vnp_HashSecret = "KK73D0NBWM5F21AVVPTMC5QIETYI2DST";

            var vnPay = new VnPayLibrary();
            var currentTime = DateTime.UtcNow.AddHours(7);
            var txnRef = booking.Id.ToString();
            var amount = (int)(booking.TotalPrice * 100);
            string userIp = "192.168.1.100";
            vnPay.AddRequestData("vnp_Amount", amount.ToString());
            vnPay.AddRequestData("vnp_Command", "pay");
            vnPay.AddRequestData("vnp_CreateDate", currentTime.ToString("yyyyMMddHHmmss"));
            vnPay.AddRequestData("vnp_CurrCode", "VND");
            vnPay.AddRequestData("vnp_ExpireDate", currentTime.AddMinutes(15).ToString("yyyyMMddHHmmss"));
            vnPay.AddRequestData("vnp_IpAddr", userIp);
            vnPay.AddRequestData("vnp_Locale", "vn");
            vnPay.AddRequestData("vnp_OrderInfo", $"DH{booking.Id}");
            vnPay.AddRequestData("vnp_OrderType", "other");
            vnPay.AddRequestData("vnp_ReturnUrl", vnp_ReturnUrl);
            vnPay.AddRequestData("vnp_TmnCode", vnp_TmnCode);
            vnPay.AddRequestData("vnp_TxnRef", txnRef);
            vnPay.AddRequestData("vnp_Version", "2.1.0");

            var paymentUrl = vnPay.CreateRequestUrl(vnp_Url, vnp_HashSecret);

            return CommonResponse<string>.Ok(paymentUrl, "Tạo URL thanh toán thành công!");
        }

        public async Task MarkAsPaidAsync(int bookingId)
        {
            var booking = await _uow.Bookings.GetByIdAsync(bookingId);
            if (booking != null && booking.Status == BookingStatus.Pending)
            {
                booking.Status = BookingStatus.Paid;
                booking.UpdatedAt = DateTime.UtcNow;
                await _uow.CompleteAsync();
            }
        }
        public async Task<CommonResponse<string>> GenerateMoMoPaymentUrlAsync(int bookingId, int userId)
        {
            var booking = await _uow.Bookings.GetAsync(b => b.Id == bookingId && b.CustomerId == userId);
            if (booking == null)
                return CommonResponse<string>.Fail("Không tìm thấy đơn hàng.");

            if (booking.Status != BookingStatus.Pending)
                return CommonResponse<string>.Fail("Đơn hàng không hợp lệ để thanh toán.");

            const string endpoint = "https://test-payment.momo.vn/v2/gateway/api/create";
            const string partnerCode = "MOMO";
            const string accessKey = "F8BBA842ECF85";
            const string secretKey = "K951B6PE1waDMi640xX08PD3vg6EkVlz";
            const string redirectUrl = "https://yourdomain.com/momo-return";
            const string ipnUrl = "https://yourdomain.com/momo-ipn";

            string orderId = $"DH{booking.Id}_{DateTime.UtcNow.Ticks}";
            string requestId = Guid.NewGuid().ToString();
            string orderInfo = $"Thanh toán đơn hàng DH{booking.Id}";
            string amount = ((int)(booking.TotalPrice)).ToString();
            string requestType = "captureWallet";
            string extraData = "";

            string rawHash = $"accessKey={accessKey}&amount={amount}&extraData={extraData}&ipnUrl={ipnUrl}&orderId={orderId}&orderInfo={orderInfo}&partnerCode={partnerCode}&redirectUrl={redirectUrl}&requestId={requestId}&requestType={requestType}";
            string signature = HmacSHA256(rawHash, secretKey);

            var body = new
            {
                partnerCode,
                accessKey,
                requestId,
                amount,
                orderId,
                orderInfo,
                redirectUrl,
                ipnUrl,
                extraData,
                requestType,
                signature,
                lang = "vi"
            };

            using var http = new HttpClient();
            var response = await http.PostAsJsonAsync(endpoint, body);
            var content = await response.Content.ReadAsStringAsync();
            Console.WriteLine("🔍 MoMo response body:");
            Console.WriteLine(content);

            using var doc = JsonDocument.Parse(content);
            var payUrl = doc.RootElement.GetProperty("payUrl").GetString();

            return CommonResponse<string>.Ok(payUrl, "Tạo URL thanh toán MoMo thành công!");
        }
        private string HmacSHA256(string text, string key)
        {
            var keyBytes = Encoding.UTF8.GetBytes(key);
            var inputBytes = Encoding.UTF8.GetBytes(text);
            using var hmac = new HMACSHA256(keyBytes);
            var hashBytes = hmac.ComputeHash(inputBytes);
            return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        }


    }
}
