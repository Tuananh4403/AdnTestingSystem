using AdnTestingSystem.Repositories.Models;

namespace AdnTestingSystem.Services.Requests
{
    public class CreateBookingRequest
    {
        public int DnaTestServiceId { get; set; }
        public int SampleMethod { get; set; }
        public int ResultTimeType { get; set; }
        public string? AppointmentDate { get; set; }
        public bool IsCivil { get; set; }
        public decimal TotalPrice { get; set; } 
    }

}
