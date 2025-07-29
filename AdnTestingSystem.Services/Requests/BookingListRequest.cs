using AdnTestingSystem.Repositories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdnTestingSystem.Services.Requests
{
    public class BookingListRequest
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public BookingStatus? Status { get; set; }
        public string? SortBy { get; set; } = "date";
        public bool SortDesc { get; set; } = true;
        public bool IsAll { get; set; } = false;
        public bool IsAppointment { get; set; } = false;
        public int? BookingId { get; set; }
        public bool IsSampleReceipt { get; set; } = false;


    }
}
