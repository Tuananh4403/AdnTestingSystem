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
    //[Authorize(Roles = "Customer")]
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
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr))
                return BadRequest("User ID không tồn tại trong token");

            var userId = int.Parse(userIdStr);
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

        [HttpGet("get-list")]
        public async Task<IActionResult> GetUserBookings([FromQuery] BookingListRequest request)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _service.GetUserBookingsAsync(userId, request);
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
        /// <summary>
        /// Cập nhật đơn đặt dịch vụ .
        /// </summary>
        /// <returns>Danh sách đơn đặt dịch vụ</returns>
        [HttpPost("update-booking")]
        [Authorize(Roles = "Staff")]
        public async Task<IActionResult> UpdateBooking([FromBody] UpdateBookingRequest request)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var result = await _service.UpdateBookingAsync(userId, request);

            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result.Message);
        }

        /// <summary>
        /// Cập nhật đơn hàng (khách hàng).
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateBookingCustomerRequest request)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr))
                return BadRequest("Không tìm thấy ID người dùng trong token.");

            var userId = int.Parse(userIdStr);
            var result = await _service.UpdateBookingCustomerAsync(userId, id, request);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Xóa mềm đơn đặt dịch vụ.
        /// </summary>
        /// <param name="id">ID đơn đặt</param>
        /// <returns>Kết quả xóa</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr))
                return BadRequest("Không tìm thấy ID người dùng trong token.");

            var userId = int.Parse(userIdStr);
            var result = await _service.SoftDeleteBookingAsync(userId, id);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("{id}/create-payment-url")]
        public async Task<IActionResult> CreatePaymentUrl(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _service.GenerateVnPayPaymentUrlAsync(id, userId);
            return result.Success ? Ok(result) : BadRequest(result);
        }

    }
}
