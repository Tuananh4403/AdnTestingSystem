using AdnTestingSystem.Repositories.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdnTestingSystem.Repositories.Models
{
    public class BookingAttachment : BaseModel
    {
        public int Id { get; set; }
        public int BookingId { get; set; }
        public string FileUrl { get; set; } = "";
        public string? Description { get; set; }
        public DateTime UploadedAt { get; set; }
        public int UploadedBy { get; set; }

        public Booking Booking { get; set; } = null!;
        public User Staff { get; set; } = null!;
    }

}
