using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdnTestingSystem.Services.Requests
{
    public class SaveTestResultRequest
    {
        public int BookingId { get; set; }
        public string Conclusion { get; set; } = string.Empty;
        public decimal CPI { get; set; }
        public decimal Probability { get; set; }
        public List<TestResultDetailRequest> LocusResults { get; set; } = new();
    }
}
