using AdnTestingSystem.Repositories.Models;
using AdnTestingSystem.Services.Interfaces;
using AdnTestingSystem.Services.Requests;
using AdnTestingSystem.Services.Responses;
using AdnTestingSystem.Services.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AdnTestingSystem.Api.Controllers
{
    [ApiController]
    [Route("api/samples")]
    [Produces("application/json")]
    [Authorize]
    public class SampleController : ControllerBase
    {
        private readonly ISampleService _service;

        public SampleController(ISampleService service)
        {
            _service = service;
        }

        /// <summary>
        /// Lấy danh sách khách hàng lấy mẫu tại nhà
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet("get-list-for-staff")]
        public async Task<IActionResult> GetBookingListForStaff([FromQuery] BookingListRequest request)
        {
            var result = await _service.GetAtHomeSampleBooking(request);
            return result.Success ? Ok(result) : BadRequest(CommonResponse<string>.Fail("Lấy dữ liệu khách hàng lấy mẫu tại nhà thất bại."));
        }


    }
}
