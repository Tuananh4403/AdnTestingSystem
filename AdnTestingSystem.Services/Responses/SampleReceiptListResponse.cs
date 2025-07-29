using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdnTestingSystem.Services.Responses
{
    public class SampleReceiptListResponse
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public int BookingId { get; set; }
        public DateTime ReceiveDate { get; set; }
        public string ReceiverName { get; set; } = string.Empty;
        public int CustomerId { get; set; }
        public string CustomerFullName { get; set; } = string.Empty;
        public int Status { get; set; }

        public List<SampleReceiptDetailResponse> Details { get; set; } = new();
    }
}
