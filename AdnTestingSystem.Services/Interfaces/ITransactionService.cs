using AdnTestingSystem.Services.Requests;
using AdnTestingSystem.Services.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdnTestingSystem.Services.Interfaces
{
    public interface ITransactionService
    {
        Task<CommonResponse<PagedResult<TransactionResponse>>> GetUserTransactionsAsync(int userId, TransactionListRequest request);
        Task<CommonResponse<List<MonthlyRevenueDto>>> GetMonthlyRevenueAsync();

    }
}
