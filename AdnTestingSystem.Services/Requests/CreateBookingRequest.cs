using AdnTestingSystem.Repositories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdnTestingSystem.Services.Requests
{
    public class CreateBookingRequest
    {
        public int DnaTestServiceId { get; set; }
        public SampleMethod SampleMethod { get; set; }
        public ResultTimeType ResultTimeType { get; set; }
        public DateTime? AppointmentTime { get; set; }
    }

}
