using AdnTestingSystem.Services.Interfaces;
using AdnTestingSystem.Services.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AdnTestingSystem.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/sample-receipt")]
    public class SampleReceiptController : ControllerBase
    {
        private readonly ISampleReceiptService _service;

        public SampleReceiptController(ISampleReceiptService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Save([FromBody] SaveSampleReceiptRequest request)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            await _service.SaveSampleReceiptAsync(request, userId);
            return Ok(new { message = "Saved successfully" });
        }

        [HttpGet("get-list")]
        public async Task<IActionResult> GetList([FromQuery] SampleReceiptListRequest request)
        {
            var result = await _service.GetSampleReceiptsAsync(request);
            return Ok(result);
        }

        [HttpPost("update-status")]
        public async Task<IActionResult> UpdateStatus([FromBody] UpdateSampleReceiptStatusRequest request)
        {
            await _service.UpdateSampleReceiptStatusAsync(request);
            return Ok(new { message = "Status updated successfully" });
        }

        [HttpGet("sample-types")]
        public async Task<IActionResult> GetSampleTypes([FromQuery] int bookingId)
        {
            var result = await _service.GetSampleTypesByBookingIdAsync(bookingId);
            return Ok(result);
        }
    }
}
