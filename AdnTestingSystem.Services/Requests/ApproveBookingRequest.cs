using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdnTestingSystem.Services.Requests
{
    public class ApproveBookingRequest
    {
        public int BookingId { get; set; }
        public int ApprovedByUserId { get; set; } 
    }
}
