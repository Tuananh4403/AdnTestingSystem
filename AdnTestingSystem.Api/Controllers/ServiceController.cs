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
        /// <param name="isCivil">true: Dân sự, false: Hành chính</param>
        /// <returns>Danh sách dịch vụ xét nghiệm đang hoạt động</returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetServices([FromQuery] bool isCivil)
            => Ok(await _service.GetServicesAsync(isCivil));

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
        [Authorize(Roles = "Manager, Admin")]
        public async Task<IActionResult> CreateService([FromBody] CreateServiceRequest req)
            => Ok(await _service.CreateServiceAsync(req));

        /// <summary>
        /// Thêm giá cho dịch vụ xét nghiệm theo hình thức và thời gian trả kết quả.
        /// </summary>
        /// <param name="id">ID dịch vụ</param>
        /// <param name="req">Thông tin giá</param>
        /// <returns>Kết quả thêm giá</returns>
        [HttpPost("{id}/prices")]
        [Authorize(Roles = "Manager, Admin")]
        public async Task<IActionResult> AddServicePrice(int id, [FromBody] AddServicePriceRequest req)
            => Ok(await _service.AddServicePriceAsync(id, req));
    }
}
