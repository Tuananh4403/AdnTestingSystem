using AdnTestingSystem.Repositories.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdnTestingSystem.Repositories.Models
{
    public class SampleReceiptSender : BaseModel
    {
        public int Id { get; set; }
        public int SampleReceiptId { get; set; }

        public string FullName { get; set; } = "";
        public DateTime? DateOfBirth { get; set; }
        public string CitizenId { get; set; } = "";
        public string Address { get; set; } = "";
        public string Phone { get; set; } = "";
        public string Relationship { get; set; } = "";

        public SampleReceipt SampleReceipt { get; set; } = null!;
    }

}
