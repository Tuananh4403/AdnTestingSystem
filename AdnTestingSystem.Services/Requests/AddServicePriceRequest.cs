using AdnTestingSystem.Repositories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdnTestingSystem.Services.Requests
{
    public class AddServicePriceRequest
    {
        public ResultTimeType ResultTimeType { get; set; }
        public SampleMethod SampleMethod { get; set; }
        public bool IsCivil { get; set; }
        public decimal Price { get; set; }
        public DateTime AppliedFrom { get; set; }
    }
}
