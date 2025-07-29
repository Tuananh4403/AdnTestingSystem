using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdnTestingSystem.Services.Requests
{
    public class UpdateSampleReceiptStatusRequest
    {
        public int SampleReceiptId { get; set; }
        public int Status { get; set; }
    }
}
