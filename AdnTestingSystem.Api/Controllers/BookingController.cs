using AdnTestingSystem.Repositories.Models;
using AdnTestingSystem.Services.Interfaces;
using AdnTestingSystem.Services.Requests;
using AdnTestingSystem.Services.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static AdnTestingSystem.Repositories.Repositories.Repository.BookingRepository;

namespace AdnTestingSystem.Api.Controllers
{
    [ApiController]
    [Route("api/bookings")]
    [Produces("application/json")]
    [Authorize(Roles = "Customer")]
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _service;

        public BookingController(IBookingService service)
        {
            _service = service;
        }

        /// <summary>
        /// Tạo đơn đặt dịch vụ xét nghiệm ADN mới.
        /// </summary>
        /// <param name="request">Thông tin dịch vụ đặt</param>
        /// <returns>Kết quả tạo đơn đặt</returns>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateBookingRequest request)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _service.CreateBookingAsync(userId, request);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Thanh toán đơn đặt dịch vụ đã tạo.
        /// </summary>
        /// <param name="id">ID đơn đặt dịch vụ</param>
        /// <returns>Kết quả thanh toán</returns>
        [HttpPost("{id}/pay")]
        public async Task<IActionResult> Pay(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _service.PayBookingAsync(id, userId);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Lấy danh sách lịch sử các đơn đã đặt của người dùng.
        /// </summary>
        /// <returns>Danh sách đơn đặt dịch vụ</returns>
        [HttpGet("history")]
        public async Task<IActionResult> History()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            return Ok(await _service.GetBookingHistoryAsync(userId));
        }

        [HttpGet("get-list-for-staff")]
        [Authorize(Roles = "Staff,Admin,Manager")]
        public async Task<IActionResult> GetBookingListForStaff([FromQuery] BookingListRequest request)
        {
            var result = await _service.GetBookingListForStaffAsync(request);
            return Ok(result);
        }

        [HttpPost("approved")]
        public async Task<IActionResult> ApproveBooking([FromBody] ApproveBookingRequest request)
        {
            var success = await _service.ApproveBookingAsync(request.BookingId, request.ApprovedByUserId);

            if (!success)
                return NotFound("Booking not found or already approved.");

            return Ok("Booking approved successfully.");
        }
    }
}
