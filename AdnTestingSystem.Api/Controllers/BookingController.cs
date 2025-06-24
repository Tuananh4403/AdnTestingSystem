// BookingController.cs
using AdnTestingSystem.Repositories.Models;
using AdnTestingSystem.Services;
using Microsoft.AspNetCore.Mvc;

namespace AdnTestingSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingController : Controller
    {
        private readonly BookingService _bookingService;

        public BookingController(BookingService bookingService)
        {
            _bookingService = bookingService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateBooking([FromBody] CreateBookingRequest request)
        {
            var bookingId = await _bookingService.CreateBookingAsync(request.CustomerId, request.ServiceId, request.SampleMethod);
            return Ok(new { bookingId });
        }
    }

    public class CreateBookingRequest
    {
        public int CustomerId { get; set; }
        public int ServiceId { get; set; }
        public SampleMethod SampleMethod { get; set; }
    }
}
