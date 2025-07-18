﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdnTestingSystem.Services.Requests
{
    public class UpdateBookingRequest
    {
        public int BookingId { get; set; }
        public string Status { get; set; }
        public string? Note { get; set; }
        public string? AttachmentImageUrl { get; set; }
    }
}
