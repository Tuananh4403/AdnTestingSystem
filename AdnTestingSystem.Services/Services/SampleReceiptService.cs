using AdnTestingSystem.Repositories.Data;
using AdnTestingSystem.Repositories.Models;
using AdnTestingSystem.Repositories.UnitOfWork;
using AdnTestingSystem.Services.Helpers;
using AdnTestingSystem.Services.Interfaces;
using AdnTestingSystem.Services.Requests;
using AdnTestingSystem.Services.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;


namespace AdnTestingSystem.Services.Services
{
    public class SampleReceiptService : ISampleReceiptService
    {
        private readonly IUnitOfWork _uow;
        private readonly AdnTestingDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SampleReceiptService(IUnitOfWork uow, AdnTestingDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _uow = uow;
            _context = context;
            _httpContextAccessor = httpContextAccessor;
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
        }

        public async Task<CommonResponse<PagedResult<SampleReceiptListResponse>>> GetSampleReceiptsAsync(SampleReceiptListRequest request)
        {
            var query = _uow.SampleReceipts.Query()
                .Include(r => r.SampleDetails)
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
                Details = r.SampleDetails.Select(d => new SampleReceiptDetailResponse
                {
                    Id = d.Id,
                    SampleType = d.SampleType,
                    Quantity = d.Quantity,
                    Status = d.Status,
                    Collector = d.Collector,
                    CollectionTime = d.CollectionTime
                }).ToList()
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

    }
}
