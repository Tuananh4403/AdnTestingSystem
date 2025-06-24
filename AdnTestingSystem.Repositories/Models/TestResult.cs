using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdnTestingSystem.Repositories.Models
{
    public class TestResult
    {
        public int Id { get; set; }
        public int BookingId { get; set; }

        public string Summary { get; set; } = "";
        public string ResultFileUrl { get; set; } = "";
        public DateTime ReleasedAt { get; set; }

        public Booking Booking { get; set; } = null!;
    }

}
