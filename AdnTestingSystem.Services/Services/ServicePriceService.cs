using AdnTestingSystem.Repositories.Models;
using AdnTestingSystem.Repositories.UnitOfWork;
using AdnTestingSystem.Services.Helpers;
using AdnTestingSystem.Services.Interfaces;
using AdnTestingSystem.Services.Requests;
using AdnTestingSystem.Services.Responses;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdnTestingSystem.Services.Services
{
    public class ServicePriceService:IServicePriceService
    {
        private readonly IUnitOfWork _uow;

        public ServicePriceService(IUnitOfWork uow)
        {
            _uow = uow;
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

        public async Task<CommonResponse<PagedResult<DnaTestServiceResponse>>> GetServicesAsync(int page, int pageSize)
        {
            var query = _uow.DnaTestServices
                .Query()
                .Where(x => x.IsActive)
                .OrderByDescending(x => x.CreatedAt);

            var paged = await PaginationHelper.ToPagedResultAsync(query, page, pageSize);

            var serviceIds = paged.Items.Select(x => x.Id).ToList();

            // Lấy toàn bộ bookings theo service id
            var bookings = await _uow.Bookings
                .Query()
                .Where(b => serviceIds.Contains(b.DnaTestServiceId))
                .Include(b => b.Rating)
                .Include(b => b.Customer)
                    .ThenInclude(u => u.Profile)
                .ToListAsync();

            var ratingsByService = bookings
                .Where(b => b.Rating != null)
                .GroupBy(b => b.DnaTestServiceId)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(b => new ServiceRatingDto
                    {
                        FullName = b.Customer.Profile?.FullName ?? "Ẩn danh",
                        Stars = b.Rating!.Stars,
                        Comment = b.Rating.Comment,
                        CreatedAt = b.Rating.CreatedAt
                    }).ToList()
                );

            var mappedItems = paged.Items.Select(service => new DnaTestServiceResponse
            {
                Id = service.Id,
                Name = service.Name,
                Description = service.Description,
                IsActive = service.IsActive,
                CreatedAt = service.CreatedAt,
                Ratings = ratingsByService.ContainsKey(service.Id)
                    ? ratingsByService[service.Id]
                    : new List<ServiceRatingDto>()
            }).ToList();

            var result = new PagedResult<DnaTestServiceResponse>
            {
                Items = mappedItems,
                TotalItems = paged.TotalItems,
                Page = paged.Page,
                PageSize = paged.PageSize
            };

            return CommonResponse<PagedResult<DnaTestServiceResponse>>.Ok(result);
        }
        public async Task<CommonResponse<string>> CreateServiceAsync(CreateServiceRequest request)
        {
            var service = new DnaTestService
            {
                Name = request.Name,
                Description = request.Description,
                IsActive = request.IsActive,
                CreatedAt = DateTime.UtcNow,
            };

            await _uow.DnaTestServices.AddAsync(service);
            await _uow.CompleteAsync();

            return CommonResponse<string>.Ok(string.Empty, "Tạo dịch vụ thành công.");
        }

        public async Task<CommonResponse<string>> AddServicePriceAsync(int serviceId, AddServicePriceRequest request)
        {
            var service = await _uow.DnaTestServices.GetByIdAsync(serviceId);
            if (service == null) return CommonResponse<string>.Fail("Không tìm thấy dịch vụ.");

            var price = new ServicePrice
            {
                DnaTestServiceId = serviceId,
                ResultTimeType = request.ResultTimeType,
                SampleMethod = request.SampleMethod,
                IsCivil = request.IsCivil,
                Price = request.Price,
                AppliedFrom = request.AppliedFrom
            };

            await _uow.ServicePrices.AddAsync(price);
            await _uow.CompleteAsync();

            return CommonResponse<string>.Ok(string.Empty, "Thêm giá cho dịch vụ thành công.");
        }
        public async Task<CommonResponse<List<ServicePriceResponse>>> GetAllServicePricesAsync()
        {
            var prices = await _uow.ServicePrices
                .Query()
                .Include(p => p.DnaTestService)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            var response = prices.Select(p => new ServicePriceResponse
            {
                Id = p.Id,
                ServiceName = p.DnaTestService.Name,
                ResultTimeType = p.ResultTimeType,
                SampleMethod = p.SampleMethod,
                IsCivil = p.IsCivil,
                Price = p.Price,
                AppliedFrom = p.AppliedFrom,
                CreatedAt = p.CreatedAt
            }).ToList();

            return CommonResponse<List<ServicePriceResponse>>.Ok(response);
        }
        public async Task<CommonResponse<decimal>> GetServicePriceAdvancedAsync(int serviceId, ResultTimeType resultTimeType, SampleMethod sampleMethod, bool isCivil)
        {
            var price = await _uow.ServicePrices.GetAsync(
                p => p.DnaTestServiceId == serviceId &&
                     p.ResultTimeType == resultTimeType &&
                     p.SampleMethod == sampleMethod &&
                     p.IsCivil == isCivil &&
                     p.AppliedFrom <= DateTime.UtcNow,
                orderBy: q => q.OrderByDescending(x => x.AppliedFrom)
            );

            return price == null
                ? CommonResponse<decimal>.Ok(0, "Vui lòng liên hệ với chúng tôi để biết thêm thông tin thanh toán!")
                : CommonResponse<decimal>.Ok(price.Price);
        }
    }
}
