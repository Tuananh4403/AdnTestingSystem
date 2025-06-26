using AdnTestingSystem.Repositories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdnTestingSystem.Services.Responses
{
    public class AtHomeSampleBookingResponse
    {
        public int BookingId { get; set; }
        public string SampleMethod { get; set; }

        public DateTime? AppointmentTime { get; set; }

        public string FullName { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }
    }
}
