using AdnTestingSystem.Repositories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdnTestingSystem.Services.Requests
{
    public class SampleReceiptListRequest
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public int? SampleReceiptId { get; set; }
        public string? CustomerFullName { get; set; }
        public SampleReceipt.SampleReceiptStatus? Status { get; set; }
    }
}
