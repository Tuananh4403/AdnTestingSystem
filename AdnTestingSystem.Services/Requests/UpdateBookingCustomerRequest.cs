using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdnTestingSystem.Services.Requests
{
    public class UpdateBookingCustomerRequest
    {
        public int DnaTestServiceId { get; set; }
        public bool IsCivil { get; set; }
        public int SampleMethod { get; set; }
        public int ResultTimeType { get; set; }
        public string? AppointmentDate { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
