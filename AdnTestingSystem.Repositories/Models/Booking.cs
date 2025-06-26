using AdnTestingSystem.Repositories.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace AdnTestingSystem.Repositories.Models
{
    public enum SampleMethod { SelfAtHome, StaffAtHome, AtClinic }
    public enum BookingStatus { Pending, Paid, KitSent, SampleCollected, InLab, Completed, Cancelled }

    public class Booking : BaseModel
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public int DnaTestServiceId { get; set; }

        public DateTime BookingDate { get; set; }
        public SampleMethod SampleMethod { get; set; }
        public BookingStatus Status { get; set; }

        public ResultTimeType ResultTimeType { get; set; } 
        public decimal TotalPrice { get; set; } 
        public DateTime? AppointmentTime { get; set; } 
        public int? ApprovedBy { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public string? Note { get; set; }
        public User Customer { get; set; } = null!;
        public DnaTestService DnaTestService { get; set; } = null!;
        public ICollection<Sample> Samples { get; set; } = new List<Sample>();
        public TestResult? TestResult { get; set; }
        public Rating? Rating { get; set; }
        public Transaction? Transaction { get; set; }
        public ICollection<BookingAttachment> BookingAttachments { get; set; } = new List<BookingAttachment>();

    }

}
