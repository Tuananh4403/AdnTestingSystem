using AdnTestingSystem.Repositories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdnTestingSystem.Services.Responses
{
    public class BookingListResponse<T>
    {

        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }

        public List<T> Items { get; set; } = new();
    }

    public class BookingStaffDto
    {
        public int Id { get; set; }
        public string CustomerName { get; set; } = null!;
        public string CustomerEmail { get; set; } = null!;
        public BookingStatus Status { get; set; }
        public DateTime BookingDate { get; set; }
        public string ServiceName { get; set; } = null!;
        public decimal TotalPrice { get; set; }
        public int CustomerId { get; set; }
        public int DnaTestServiceId { get; set; }
    }

}
