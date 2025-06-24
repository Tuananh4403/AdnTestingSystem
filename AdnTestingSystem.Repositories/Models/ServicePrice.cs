using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdnTestingSystem.Repositories.Models
{
    public class ServicePrice
    {
        public int Id { get; set; }
        public int DnaTestServiceId { get; set; }

        public decimal Price { get; set; }
        public DateTime AppliedFrom { get; set; }

        public DnaTestService DnaTestService { get; set; } = null!;
    }

}
