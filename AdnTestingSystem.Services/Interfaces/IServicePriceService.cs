using AdnTestingSystem.Repositories.Models;
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
        Task<CommonResponse<IEnumerable<DnaTestService>>> GetServicesAsync(bool isCivil);
        Task<CommonResponse<decimal>> GetServicePriceAsync(int serviceId, ResultTimeType resultType, SampleMethod sampleMethod);
        Task<CommonResponse<string>> CreateServiceAsync(CreateServiceRequest request);
        Task<CommonResponse<string>> AddServicePriceAsync(int serviceId, AddServicePriceRequest request);
    }
}
