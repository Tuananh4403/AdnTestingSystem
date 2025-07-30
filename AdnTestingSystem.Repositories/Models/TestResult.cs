using AdnTestingSystem.Repositories.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdnTestingSystem.Repositories.Models
{
    public class TestResult : BaseModel
    {
        public enum TestResultStatus
        {
            Pending = 0,
            Approved = 1
        }
        public int Id { get; set; }

        public int BookingId { get; set; }
        public Booking Booking { get; set; } = null!;

        public string Conclusion { get; set; } = string.Empty;

        public decimal CPI { get; set; } 
        public decimal Probability { get; set; } 

        public int CreatedById { get; set; }
        public User CreatedBy { get; set; } = null!;
        public TestResultStatus Status { get; set; } = TestResultStatus.Pending;
        public ICollection<TestResultDetail> LocusResults { get; set; } = new List<TestResultDetail>();
    }

}
