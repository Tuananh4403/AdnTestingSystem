using AdnTestingSystem.Services.Interfaces;
using AdnTestingSystem.Services.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AdnTestingSystem.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/test-result")]
    public class TestResultController : ControllerBase
    {
        private readonly ITestResultService _service;

        public TestResultController(ITestResultService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Save([FromBody] SaveTestResultRequest request)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            await _service.SaveTestResultAsync(request, userId);
            return Ok(new { message = "Test result saved successfully" });
        }
        [HttpGet("get-list")]
        public async Task<IActionResult> GetList([FromQuery] TestResultListRequest request)
        {
            var result = await _service.GetTestResultsAsync(request);
            return Ok(result);
        }

    }
}
