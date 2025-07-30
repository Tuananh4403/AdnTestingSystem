using AdnTestingSystem.Repositories.Models;
using AdnTestingSystem.Services.Helpers;
using AdnTestingSystem.Services.Requests;
using AdnTestingSystem.Services.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdnTestingSystem.Services.Interfaces
{
    public interface IServicePriceService
    {
        Task<CommonResponse<decimal>> GetServicePriceAsync(int serviceId, ResultTimeType resultType, SampleMethod sampleMethod);
        Task<CommonResponse<PagedResult<DnaTestServiceResponse>>> GetServicesAsync(int page, int pageSize);
        Task<CommonResponse<string>> CreateServiceAsync(CreateServiceRequest request);
        Task<CommonResponse<string>> AddServicePriceAsync(int serviceId, AddServicePriceRequest request);
        Task<CommonResponse<List<ServicePriceResponse>>> GetAllServicePricesAsync();
        Task<CommonResponse<decimal>> GetServicePriceAdvancedAsync(int serviceId, ResultTimeType resultTimeType, SampleMethod sampleMethod, bool isCivil);
        Task<CommonResponse<List<ServiceRatingStatisticDto>>> GetRatingStatisticsAsync(int? month = null);


    }
}
