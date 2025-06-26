using AdnTestingSystem.Repositories.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdnTestingSystem.Repositories.Models
{
    public enum BioSampleType { Blood, Saliva, Hair, Nail, UmbilicalCord }

    public class SampleDetail : BaseModel
    {
        public int Id { get; set; }
        public int SampleReceiptId { get; set; }

        public BioSampleType Type { get; set; }
        public int Quantity { get; set; }
        public DateTime CollectedAt { get; set; }
        public string Condition { get; set; } = "";
        public string CollectedBy { get; set; } = "";

        public SampleReceipt SampleReceipt { get; set; } = null!;
    }

}
