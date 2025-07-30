using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdnTestingSystem.Services.Requests
{
    public class SampleDetailRequest
    {
        public string Type { get; set; }
        public int Quantity { get; set; }
        public string Status { get; set; }
        public string Collector { get; set; }
        public string Owner { get; set; }
        public string Relationship { get; set; }
        public string SampleCode { get; set; }
        public DateTime CollectionTime { get; set; }
    }
}
