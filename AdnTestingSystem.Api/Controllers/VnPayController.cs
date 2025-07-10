using AdnTestingSystem.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdnTestingSystem.Api.Controllers
{
    public class VnPayController : ControllerBase
    {
        private readonly IBookingService _service;

        public VnPayController(IBookingService service)
        {
            _service = service;
        }
        [HttpGet("/vnpay-return")]
        [AllowAnonymous]
        public async Task<IActionResult> VnPayReturn()
        {
            var query = Request.Query;
            var vnp_ResponseCode = query["vnp_ResponseCode"];
            var vnp_TxnRef = query["vnp_TxnRef"];

            if (vnp_ResponseCode == "00") 
            {
                int bookingId = int.Parse(vnp_TxnRef!);
                await _service.MarkAsPaidAsync(bookingId);
                return Redirect("https://localhost:5173/my-orders");
            }
            return Redirect("https://localhost:5173/my-orders");
        }

    }
}
