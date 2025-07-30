using AdnTestingSystem.Services.Interfaces;
using AdnTestingSystem.Services.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AdnTestingSystem.Api.Controllers
{
    [ApiController]
    [Route("api/transactions")]
    [Produces("application/json")]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionService _transactionService;

        public TransactionController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        /// <summary>
        /// Lấy danh sách giao dịch thanh toán theo người dùng có phân trang và lọc trạng thái.
        /// </summary>
        [HttpGet("history")]
        public async Task<IActionResult> GetTransactionHistory([FromQuery] TransactionListRequest request)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr))
                return BadRequest("Không tìm thấy ID người dùng trong token.");

            var userId = int.Parse(userIdStr);
            var result = await _transactionService.GetUserTransactionsAsync(userId, request);
            return Ok(result);
        }
        /// <summary>
        /// Thống kê tổng tiền thu được mỗi tháng từ các giao dịch đã thanh toán.
        /// </summary>
        [HttpGet("monthly-revenue")]
        [Authorize(Roles = "Admin")] // Chỉ admin được phép xem
        public async Task<IActionResult> GetMonthlyRevenue()
        {
            var result = await _transactionService.GetMonthlyRevenueAsync();
            return Ok(result);
        }

    }
}
