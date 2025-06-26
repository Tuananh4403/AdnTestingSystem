using AdnTestingSystem.Services.Interfaces;
using AdnTestingSystem.Services.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AdnTestingSystem.Api.Controllers
{
    [ApiController]
    [Route("api/rating")]
    public class RatingController : ControllerBase
    {
        private readonly IRatingService _ratingService;

        public RatingController(IRatingService ratingService)
        {
            _ratingService = ratingService;
        }
        /// <summary>
        /// Tạo đánh giá (feedback) cho một booking đã hoàn tất.
        /// </summary>
        /// <param name="request">Thông tin đánh giá: BookingId, Stars, Comment.</param>
        /// <returns>Kết quả tạo đánh giá thành công hoặc thất bại.</returns>
        /// <remarks>Chỉ khách hàng có quyền và chỉ đánh giá một lần trên mỗi booking.</remarks>

        [HttpPost("new-rating")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> NewRating([FromBody] NewRatingRequest request)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var response = await _ratingService.CreateRatingAsync(userId, request);
            return response.Success ? Ok(response) : BadRequest(response);
        }
        /// <summary>
        /// Lấy đánh giá (rating) theo mã booking.
        /// </summary>
        /// <param name="bookingId">Mã booking cần lấy đánh giá.</param>
        /// <returns>Thông tin đánh giá của booking (nếu có).</returns>

        [HttpGet("list-rating")]
        public async Task<IActionResult> GetRatingByBookingId([FromQuery] int bookingId)
        {
            var result = await _ratingService.GetRatingByBookingIdAsync(bookingId);
            return result.Success ? Ok(result.Data) : NotFound(result.Message);
        }
    }

}
