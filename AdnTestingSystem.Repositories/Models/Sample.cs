﻿using AdnTestingSystem.Repositories.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdnTestingSystem.Repositories.Models
{
    public class Sample : BaseModel
    {
        public int Id { get; set; }
        public int BookingId { get; set; }

        public string SampleCode { get; set; } = "";
        public DateTime CollectedAt { get; set; }
        public string CollectedBy { get; set; } = "";
        public string? CollectorPhone { get; set; } = null; 
        public string? CollectorTitle { get; set; } = null;

        public Booking Booking { get; set; } = null!;
    }

}
