using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdnTestingSystem.Services.Responses
{
    public class SampleReceiptListResponse
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public int BookingId { get; set; }
        public DateTime ReceiveDate { get; set; }
        public string ReceiverName { get; set; } = string.Empty;
        public int CustomerId { get; set; }
        public string CustomerFullName { get; set; } = string.Empty;
        public int Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public BookingResponse? Booking { get; set; }
        public CustomerResponse? Customer { get; set; }

        public List<SampleReceiptDetailResponse> Details { get; set; } = new();
    }
    public class BookingResponse
    {
        public int Id { get; set; }
        public DateTime BookingDate { get; set; }
        public int CustomerId { get; set; }
        public int DnaTestServiceId { get; set; }
        public string DnaTestServiceName { get; set; } = "";
        public int SampleMethod { get; set; }
        public int Status { get; set; }
        public bool IsCivil { get; set; }
        public DateTime? AppointmentTime { get; set; }
    }

    public class CustomerResponse
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
    }
}
