using AdnTestingSystem.Repositories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdnTestingSystem.Services.Responses
{
    public class TransactionResponse
    {
        public int Id { get; set; }
        public int BookingId { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; } = "";
        public string TransactionCode { get; set; } = "";
        public PaymentStatus Status { get; set; }
        public string Message { get; set; } = "";
        public DateTime CreatedAt { get; set; }
    }
}
