using AdnTestingSystem.Repositories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdnTestingSystem.Services.Requests
{
    public class TransactionListRequest
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public PaymentStatus? Status { get; set; }
    }
}
