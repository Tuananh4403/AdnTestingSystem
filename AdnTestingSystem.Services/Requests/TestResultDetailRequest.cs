using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdnTestingSystem.Services.Requests
{
    public class TestResultDetailRequest
    {
        public string Locus { get; set; } = string.Empty;
        public string Allele1_Person1 { get; set; } = string.Empty;
        public string Allele2_Person1 { get; set; } = string.Empty;
        public string Allele1_Person2 { get; set; } = string.Empty;
        public string Allele2_Person2 { get; set; } = string.Empty;
        public decimal PI { get; set; }
        public string? Note { get; set; }
    }
}
