using AdnTestingSystem.Repositories.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdnTestingSystem.Repositories.Models
{
    public class DnaTestService : Model
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public bool IsCivil { get; set; }
        public bool IsActive { get; set; }

        public ICollection<ServicePrice> Prices { get; set; } = new List<ServicePrice>();
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }

}
