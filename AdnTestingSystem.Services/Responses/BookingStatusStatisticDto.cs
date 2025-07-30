using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdnTestingSystem.Services.Responses
{
    public class BookingStatusStatisticDto
    {
        public int TotalBookings { get; set; }
        public int CompletedCount { get; set; }
        public double CompletedPercentage { get; set; }

        public int CancelledCount { get; set; }
        public double CancelledPercentage { get; set; }

        public int ProcessingCount { get; set; }
        public double ProcessingPercentage { get; set; }
    }
}
