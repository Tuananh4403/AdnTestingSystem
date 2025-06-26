using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdnTestingSystem.Services.Requests
{
    public class NewRatingRequest
    {
        public int BookingId { get; set; }
        public int Stars { get; set; }
        public string? Comment { get; set; }
    }

}
