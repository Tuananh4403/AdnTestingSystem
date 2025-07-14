using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdnTestingSystem.Repositories.Models
{
    public enum PaymentStatus { Pending, Paid, Failed }

    public class Transaction
    {
        public int Id { get; set; }
        public int BookingId { get; set; }

        public string PaymentMethod { get; set; } = ""; 
        public string TransactionCode { get; set; } = ""; 
        public PaymentStatus Status { get; set; }

        public decimal Amount { get; set; }
        public int CreatedBy { get; set; } 
        public string? Message { get; set; }
        public DateTime CreatedAt { get; set; }

        public Booking Booking { get; set; } = null!;
    }

}
