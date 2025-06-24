using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdnTestingSystem.Repositories.Models
{
    public class Rating
    {
        public int Id { get; set; }
        public int BookingId { get; set; }

        public int Stars { get; set; }
        public string Comment { get; set; } = "";
        public DateTime CreatedAt { get; set; }

        public Booking Booking { get; set; } = null!;
    }

}
