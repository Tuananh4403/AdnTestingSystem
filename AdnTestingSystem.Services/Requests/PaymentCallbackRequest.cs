using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdnTestingSystem.Services.Requests
{
    public class PaymentCallbackRequest
    {
        public int OrderId { get; set; }
        public string ResponseCode { get; set; } = default!;
    }
}
