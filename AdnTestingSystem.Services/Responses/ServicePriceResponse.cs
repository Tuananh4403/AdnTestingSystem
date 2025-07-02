using AdnTestingSystem.Repositories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdnTestingSystem.Services.Responses
{
    public class ServicePriceResponse
    {
        public int Id { get; set; }
        public string ServiceName { get; set; } = string.Empty;
        public ResultTimeType ResultTimeType { get; set; }
        public SampleMethod SampleMethod { get; set; }
        public bool IsCivil { get; set; }
        public decimal Price { get; set; }
        public DateTime AppliedFrom { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
