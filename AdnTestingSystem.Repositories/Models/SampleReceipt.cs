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
        public enum TestType
        {
            FatherChild,
            MotherChild,
            Siblings,
            Identity,
            Forensic
        }

        public int Id { get; set; }
        public int BookingId { get; set; }

        public TestType Type { get; set; }

        public string? CommitmentNote { get; set; }
        public bool IsLegalConfirmed { get; set; }
        public bool IsUsedForDnaTest { get; set; }

        public string? SpecialRequest { get; set; }

        public string? ResultReceiverName { get; set; }
        public string? ResultReceiverPhone { get; set; }
        public string? ResultReceiverEmail { get; set; }
        public string? ResultReceiverAddress { get; set; }
        public string? ResultReceiveMethod { get; set; }

        public DateTime ReceivedAt { get; set; }
        public string ReceivedByName { get; set; } = "";
        public string DeliveredByName { get; set; } = "";

        public Booking Booking { get; set; } = null!;
        public ICollection<SampleReceiptSender> Senders { get; set; } = new List<SampleReceiptSender>();
        public ICollection<SampleDetail> Samples { get; set; } = new List<SampleDetail>();
    }

}
