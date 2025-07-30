using AdnTestingSystem.Repositories.Models;
using AdnTestingSystem.Services.Interfaces;
using AdnTestingSystem.Services.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdnTestingSystem.Api.Controllers
{
    [ApiController]
    [Route("api/services")]
    [Produces("application/json")]
    public class ServiceController : ControllerBase
    {
        private readonly IServicePriceService _service;

        public ServiceController(IServicePriceService service)
        {
            _service = service;
        }

        /// <summary>
        /// Lấy danh sách dịch vụ xét nghiệm ADN theo loại (Dân sự hoặc Hành chính).
        /// </summary>
        /// <param name="isActive">true:Mặc định là true</param>
        /// <returns>Danh sách dịch vụ xét nghiệm đang hoạt động</returns>
        [HttpGet("get-list-services")]
        public async Task<IActionResult> GetPagedServices([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
         => Ok(await _service.GetServicesAsync(page, pageSize));

        /// <summary>
        /// Lấy giá dịch vụ theo hình thức lấy mẫu và thời gian trả kết quả.
        /// </summary>
        /// <param name="id">ID dịch vụ xét nghiệm</param>
        /// <param name="resultTimeType">Thời gian trả kết quả</param>
        /// <param name="sampleMethod">Hình thức lấy mẫu</param>
        /// <returns>Giá tiền tương ứng</returns>
        [HttpGet("{id}/price")]
        [AllowAnonymous]
        public async Task<IActionResult> GetPrice(
            int id,
            [FromQuery] ResultTimeType resultTimeType,
            [FromQuery] SampleMethod sampleMethod)
            => Ok(await _service.GetServicePriceAsync(id, resultTimeType, sampleMethod));

        /// <summary>
        /// Tạo mới một dịch vụ xét nghiệm ADN.
        /// </summary>
        /// <param name="req">Thông tin dịch vụ</param>
        /// <returns>Kết quả tạo dịch vụ</returns>
        [HttpPost]
        public async Task<IActionResult> CreateService([FromBody] CreateServiceRequest req)
            => Ok(await _service.CreateServiceAsync(req));

        /// <summary>
        /// Thêm giá cho dịch vụ xét nghiệm theo hình thức và thời gian trả kết quả.
        /// </summary>
        /// <param name="id">ID dịch vụ</param>
        /// <param name="req">Thông tin giá</param>
        /// <returns>Kết quả thêm giá</returns>
        [HttpPost("{id}/prices")]
        public async Task<IActionResult> AddServicePrice(int id, [FromBody] AddServicePriceRequest req)
            => Ok(await _service.AddServicePriceAsync(id, req));

        /// <summary>
        /// Lấy danh sách tất cả giá dịch vụ.
        /// </summary>
        /// <returns>Danh sách giá dịch vụ kèm tên dịch vụ</returns>
        [HttpGet("prices")]
        public async Task<IActionResult> GetAllPrices()
            => Ok(await _service.GetAllServicePricesAsync());

        /// <summary>
        /// Lấy giá dịch vụ theo loại xét nghiệm, phương thức lấy mẫu và thời gian trả kết quả.
        /// </summary>
        /// <param name="id">ID dịch vụ</param>
        /// <param name="resultTimeType">Thời gian trả kết quả</param>
        /// <param name="sampleMethod">Phương thức lấy mẫu</param>
        /// <param name="isCivil">Loại xét nghiệm (Dân sự / Hành chính)</param>
        /// <returns>Giá tiền phù hợp nếu có</returns>
        [HttpGet("{id}/advanced-price")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAdvancedPrice(
            int id,
            [FromQuery] ResultTimeType resultTimeType,
            [FromQuery] SampleMethod sampleMethod,
            [FromQuery] bool isCivil)
        {
            return Ok(await _service.GetServicePriceAdvancedAsync(id, resultTimeType, sampleMethod, isCivil));
        }
        [HttpGet("rating-statistics")]
        public async Task<IActionResult> GetRatingStatistics([FromQuery] int? month = null)
        {
            var result = await _service.GetRatingStatisticsAsync(month);
            return Ok(result);
        }

    }
}
