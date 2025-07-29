using AdnTestingSystem.Repositories.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdnTestingSystem.Repositories.Models
{
    public class SampleReceipt : BaseModel
    {
        public enum SampleReceiptStatus
        {
            Unconfirmed = 0,
            Confirmed = 1
        }

        public int Id { get; set; }
        public string Code => $"PTM{Id}";
        public int BookingId { get; set; }
        public Booking Booking { get; set; } = null!;

        public int CreatedById { get; set; }
        public User CreatedBy { get; set; } = null!;

        public DateTime ReceiveDate { get; set; }
        public string ReceiverName { get; set; } = string.Empty;

        public SampleReceiptStatus Status { get; set; } = SampleReceiptStatus.Unconfirmed;

        public int CustomerId { get; set; }
        public string CustomerFullName { get; set; } = string.Empty;

        public ICollection<SampleReceiptDetail> SampleDetails { get; set; } = new List<SampleReceiptDetail>();
    }

}
