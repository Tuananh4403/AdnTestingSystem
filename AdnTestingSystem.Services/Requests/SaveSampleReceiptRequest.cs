using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdnTestingSystem.Services.Requests
{
    public class SaveSampleReceiptRequest
    {
        public int BookingId { get; set; }
        public int CustomerId { get; set; }
        public string CustomerFullName { get; set; }
        public string ReceiverName { get; set; }
        public DateTime ReceiveDate { get; set; }
        public List<SampleDetailRequest> Samples { get; set; }
    }
}
