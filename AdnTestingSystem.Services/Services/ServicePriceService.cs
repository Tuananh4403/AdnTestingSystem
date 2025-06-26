using AdnTestingSystem.Repositories.Models;
using AdnTestingSystem.Repositories.UnitOfWork;
using AdnTestingSystem.Services.Interfaces;
using AdnTestingSystem.Services.Requests;
using AdnTestingSystem.Services.Responses;
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

        public async Task<CommonResponse<string>> CreateServiceAsync(CreateServiceRequest request)
        {
            var service = new DnaTestService
            {
                Name = request.Name,
                Description = request.Description,
                IsCivil = request.IsCivil,
                IsActive = true
            };

            await _uow.DnaTestServices.AddAsync(service);
            await _uow.CompleteAsync();

            return CommonResponse<string>.Ok("Tạo dịch vụ thành công.");
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

            return CommonResponse<string>.Ok("Thêm giá cho dịch vụ thành công.");
        }
    }
}
