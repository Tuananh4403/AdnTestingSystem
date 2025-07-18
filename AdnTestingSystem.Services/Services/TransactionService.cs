using AdnTestingSystem.Repositories.UnitOfWork;
using AdnTestingSystem.Services.Helpers;
using AdnTestingSystem.Services.Interfaces;
using AdnTestingSystem.Services.Requests;
using AdnTestingSystem.Services.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdnTestingSystem.Services.Services
{
    public class TransactionService: ITransactionService
    {
        private readonly IUnitOfWork _uow;

        public TransactionService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<CommonResponse<PagedResult<TransactionResponse>>> GetUserTransactionsAsync(int userId, TransactionListRequest request)
        {
            request.PageSize = request.PageSize <= 0 ? 20 : request.PageSize;
            request.Page = request.Page <= 0 ? 1 : request.Page;

            var query = _uow.Transactions.Query()
                .Where(t => t.CreatedBy == userId);

            if (request.Status.HasValue)
                query = query.Where(t => t.Status == request.Status);

            query = query.OrderByDescending(t => t.CreatedAt);

            var paged = await PaginationHelper.ToPagedResultAsync(query, request.Page, request.PageSize);

            var results = paged.Items.Select(t => new TransactionResponse
            {
                Id = t.Id,
                BookingId = t.BookingId,
                Amount = t.Amount,
                PaymentMethod = t.PaymentMethod,
                TransactionCode = t.TransactionCode,
                Status = t.Status,
                Message = t.Message,
                CreatedAt = t.CreatedAt
            }).ToList();

            return CommonResponse<PagedResult<TransactionResponse>>.Ok(new PagedResult<TransactionResponse>
            {
                Items = results,
                Page = paged.Page,
                PageSize = paged.PageSize,
                TotalItems = paged.TotalItems
            });
        }
    }
}
