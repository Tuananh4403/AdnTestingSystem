using AdnTestingSystem.Repositories.Data;
using AdnTestingSystem.Repositories.Models;
using AdnTestingSystem.Repositories.UnitOfWork;
using AdnTestingSystem.Services.Helpers;
using AdnTestingSystem.Services.Interfaces;
using AdnTestingSystem.Services.Requests;
using AdnTestingSystem.Services.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Text;


namespace AdnTestingSystem.Services.Services
{
    public class SampleReceiptService : ISampleReceiptService
    {
        private readonly IUnitOfWork _uow;
        private readonly AdnTestingDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IEmailSender _email;

        public SampleReceiptService(IUnitOfWork uow, AdnTestingDbContext context, IHttpContextAccessor httpContextAccessor, IEmailSender email)
        {
            _uow = uow;
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _email = email;
        }

        public async Task SaveSampleReceiptAsync(SaveSampleReceiptRequest request, int currentUserId)
        {
            var now = DateTime.UtcNow;

            var sampleReceipt = new SampleReceipt
            {
                BookingId = request.BookingId,
                CustomerId = request.CustomerId,
                CustomerFullName = request.CustomerFullName,
                ReceiverName = request.ReceiverName,
                ReceiveDate = request.ReceiveDate,

                CreatedById = currentUserId,
                CreatedAt = now,
                UpdatedBy = currentUserId,
                UpdatedAt = now,

                Status = SampleReceipt.SampleReceiptStatus.Unconfirmed
            };

            foreach (var sample in request.Samples)
            {
                var detail = new SampleReceiptDetail
                {
                    SampleType = sample.Type,
                    Quantity = sample.Quantity,
                    Status = sample.Status,
                    Collector = sample.Collector,
                    Owner = sample.Owner,
                    Relationship = sample.Relationship,
                    SampleCode = sample.SampleCode,
                    CollectionTime = sample.CollectionTime,
                    CreatedAt = now,
                    CreatedBy = currentUserId,
                    UpdatedBy = currentUserId,
                    UpdatedAt = now
                };

                sampleReceipt.SampleDetails.Add(detail);
            }

            _context.SampleReceipts.Add(sampleReceipt);
            await _context.SaveChangesAsync();
            await SendSampleReceiptNotificationEmailAsync(request.CustomerFullName);
            var booking = await _context.Bookings.FirstOrDefaultAsync(b => b.Id == request.BookingId);
            if (booking != null)
            {
                booking.IsSampleReceiptCreated = true;
                booking.UpdatedAt = DateTime.UtcNow;
                booking.UpdatedBy = currentUserId;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<CommonResponse<PagedResult<SampleReceiptListResponse>>> GetSampleReceiptsAsync(SampleReceiptListRequest request)
        {
            var query = _uow.SampleReceipts.Query()
                .Include(r => r.SampleDetails)
                .Include(r => r.Booking)
                    .ThenInclude(b => b.Customer)       
                        .ThenInclude(c => c.Profile)
                .Include(r => r.Booking)            
                    .ThenInclude(b => b.DnaTestService)
                .Where(r => r.DeletedAt == null);

            if (request.SampleReceiptId.HasValue)
            {
                query = query.Where(r => r.Id == request.SampleReceiptId.Value);
            }

            if (!string.IsNullOrWhiteSpace(request.CustomerFullName))
            {
                query = query.Where(r => r.CustomerFullName.Contains(request.CustomerFullName));
            }

            if (request.Status.HasValue)
            {
                query = query.Where(r => r.Status == request.Status.Value);
            }

            query = query.OrderByDescending(r => r.UpdatedAt);

            var paged = await PaginationHelper.ToPagedResultAsync(query, request.Page, request.PageSize);

            var items = paged.Items.Select(r => new SampleReceiptListResponse
            {
                Id = r.Id,
                Code = r.Code,
                BookingId = r.BookingId,
                ReceiveDate = r.ReceiveDate,
                ReceiverName = r.ReceiverName,
                CustomerId = r.CustomerId,
                CustomerFullName = r.CustomerFullName,
                Status = (int)r.Status,
                CreatedAt = r.CreatedAt,

                Details = r.SampleDetails.Select(d => new SampleReceiptDetailResponse
                {
                    Id = d.Id,
                    SampleType = d.SampleType,
                    Quantity = d.Quantity,
                    Status = d.Status,
                    Collector = d.Collector,
                    Owner = d.Owner,
                    SampleCode = d.SampleCode,
                    Relationship = d.Relationship,
                    CollectionTime = d.CollectionTime
                }).ToList(),

                Booking = r.Booking == null ? null : new BookingResponse
                {
                    Id = r.Booking.Id,
                    BookingDate = r.Booking.BookingDate,
                    CustomerId = r.Booking.CustomerId,
                    DnaTestServiceId = r.Booking.DnaTestServiceId,
                    DnaTestServiceName = r.Booking.DnaTestService?.Name ?? "",
                    SampleMethod = (int)r.Booking.SampleMethod,
                    Status = (int)r.Booking.Status,
                    IsCivil = r.Booking.IsCivil,
                    AppointmentTime = r.Booking.AppointmentTime
                },

                Customer = r.Booking?.Customer == null ? null : new CustomerResponse
                {
                    Id = r.Booking.Customer.Id,
                    Email = r.Booking.Customer.Email,
                    FullName = r.Booking.Customer.Profile?.FullName ?? "",
                    Phone = r.Booking.Customer.Profile?.Phone ?? "",
                    Address = r.Booking.Customer.Profile?.Address ?? ""
                }
            }).ToList();

            return CommonResponse<PagedResult<SampleReceiptListResponse>>.Ok(new PagedResult<SampleReceiptListResponse>
            {
                Items = items,
                TotalItems = paged.TotalItems,
                Page = paged.Page,
                PageSize = paged.PageSize
            });
        }

        public async Task UpdateSampleReceiptStatusAsync(UpdateSampleReceiptStatusRequest request)
        {
            var receipt = await _context.SampleReceipts
                .FirstOrDefaultAsync(r => r.Id == request.SampleReceiptId && r.DeletedAt == null);

            if (receipt == null)
                throw new Exception("SampleReceipt not found");

            receipt.Status = (SampleReceipt.SampleReceiptStatus)request.Status;
            receipt.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }

        private async Task SendSampleReceiptNotificationEmailAsync(string customerFullName)
        {
            var user = await _context.Users
                .Include(u => u.Profile)
                .FirstOrDefaultAsync(u => u.Profile != null && u.Profile.FullName == customerFullName);

            if (user == null || string.IsNullOrWhiteSpace(user.Email))
            {
                return;
            }

            var emailBody = new StringBuilder();
            emailBody.AppendLine("<p>Kính chào quý khách,</p>");
            emailBody.AppendLine("<p>MATAP xin thông báo chúng tôi đã nhận được mẫu xét nghiệm bạn gửi.</p>");
            emailBody.AppendLine("<p>Vui lòng vào phần mềm tại mục <strong>Xác nhận mẫu đã gửi</strong> để tiến hành xác nhận thông tin!</p>");
            emailBody.AppendLine("<p>Trân trọng,<br/>Admin</p>");

            await _email.SendAsync(user.Email, "Thông báo nhận mẫu xét nghiệm", emailBody.ToString());

        }

        public async Task<List<SampleTypeResponse>> GetSampleTypesByBookingIdAsync(int bookingId)
        {
            var receipt = await _context.SampleReceipts
                .Include(r => r.SampleDetails)
                .FirstOrDefaultAsync(r => r.BookingId == bookingId && r.DeletedAt == null);

            if (receipt == null)
                throw new Exception("SampleReceipt not found");

            return receipt.SampleDetails
                .Select(d => new SampleTypeResponse
                {
                    SampleType = d.SampleType
                })
                .ToList();
        }

    }
}
