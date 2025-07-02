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
            .OrderByDescending(x => x.CreatedAt);

            var paged = await PaginationHelper.ToPagedResultAsync(query, page, pageSize);

            var creatorIds = paged.Items.Select(x => x.CreatedBy).Distinct().ToList();
            var creators = await _uow.Users
            .Query()
            .Include(u => u.Profile)
            .Where(u => creatorIds.Contains(u.Id))
            .ToListAsync();

            var creatorDict = creators.ToDictionary(
                u => u.Id,
                u => u.Profile?.FullName ?? "Không rõ"
            );


            var mappedItems = paged.Items.Select(service => new DnaTestServiceResponse
            {
                Id = service.Id,
                Name = service.Name,
                Description = service.Description,
                IsActive = service.IsActive,
                CreatedAt = service.CreatedAt,

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

    }
}
