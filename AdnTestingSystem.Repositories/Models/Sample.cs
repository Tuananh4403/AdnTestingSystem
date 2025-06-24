using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdnTestingSystem.Repositories.Models
{
    public class Sample
    {
        public int Id { get; set; }
        public int BookingId { get; set; }

        public string SampleCode { get; set; } = "";
        public DateTime CollectedAt { get; set; }
        public string CollectedBy { get; set; } = "";

        public Booking Booking { get; set; } = null!;
    }

}
