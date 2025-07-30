using AdnTestingSystem.Repositories.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdnTestingSystem.Repositories.Models
{
    public class SampleReceiptDetail : BaseModel
    {
        public int Id { get; set; }

        public int SampleReceiptId { get; set; }
        public SampleReceipt SampleReceipt { get; set; } = null!;

        public string SampleType { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public string Status { get; set; } = string.Empty;

        public string Collector { get; set; } = string.Empty;
        public string Owner { get; set; } = string.Empty;
        public string Relationship { get; set; } = string.Empty;
        public string SampleCode { get; set; } = string.Empty;
        public DateTime CollectionTime { get; set; }
    }

}
