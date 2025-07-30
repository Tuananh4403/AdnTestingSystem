using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdnTestingSystem.Services.Responses
{
    public class TestResultListResponse
    {
        public int Id { get; set; }
        public int BookingId { get; set; }
        public string Conclusion { get; set; }
        public decimal CPI { get; set; }
        public decimal Probability { get; set; }
        public int Status { get; set; }
        public DateTime CreatedAt { get; set; }

        public List<TestResultDetailResponse> Details { get; set; } = new();
    }

    public class TestResultDetailResponse
    {
        public int Id { get; set; }
        public string Locus { get; set; }
        public string Allele1_Person1 { get; set; }
        public string Allele2_Person1 { get; set; }
        public string Allele1_Person2 { get; set; }
        public string Allele2_Person2 { get; set; }
        public decimal PI { get; set; }
        public string? Note { get; set; }
    }
}
