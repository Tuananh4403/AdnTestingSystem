using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdnTestingSystem.Services.Responses
{
    public class RatingViewModel
    {
        public int Id { get; set; }
        public string CustomerName { get; set; } = "";
        public string ServiceName { get; set; } = "";
        public int Stars { get; set; }
        public string Comment { get; set; } = "";
        public DateTime CreatedAt { get; set; }
    }
}

