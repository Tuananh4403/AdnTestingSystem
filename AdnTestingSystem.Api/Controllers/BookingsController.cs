using AdnTestingSystem.Repositories.Models;
using AdnTestingSystem.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using static AdnTestingSystem.Repositories.AdnTestingSystem.Repositories.Implementations.BookingRepository;

namespace AdnTestingSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingsController : Controller
    {
        private readonly BookingService _bookingService;

        public BookingsController(BookingService bookingService)
        {
            _bookingService = bookingService;
        }
    
        [HttpGet("get-list-for-staff")]
        //[Authorize(Roles = "Staff,Admin,Manager")]
        public async Task<IActionResult> GetBookingListForStaff([FromQuery] BookingListRequest request)
        {
            var result = await _bookingService.GetBookingListForStaffAsync(request);
            return Ok(result);
        }
    }

}
