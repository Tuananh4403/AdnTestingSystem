﻿using AdnTestingSystem.Repositories.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdnTestingSystem.Repositories.Models
{
    public enum ResultTimeType { Express, Fast, Normal }

    public class ServicePrice : Model
    {
        public int Id { get; set; }
        public int DnaTestServiceId { get; set; }

        public ResultTimeType ResultTimeType { get; set; }
        public SampleMethod SampleMethod { get; set; }
        public bool IsCivil { get; set; }

        public decimal Price { get; set; }
        public DateTime AppliedFrom { get; set; }

        public DnaTestService DnaTestService { get; set; } = null!;
    }

}
