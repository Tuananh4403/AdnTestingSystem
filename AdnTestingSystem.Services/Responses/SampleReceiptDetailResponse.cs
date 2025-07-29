using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdnTestingSystem.Services.Responses
{
    public class SampleReceiptDetailResponse
    {
        public int Id { get; set; }
        public string SampleType { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Collector { get; set; } = string.Empty;
        public DateTime CollectionTime { get; set; }
    }
}
