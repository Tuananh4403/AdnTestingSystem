﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdnTestingSystem.Services.Responses
{
    public class ServiceRatingDto
    {
        public string FullName { get; set; } = "";
        public int Stars { get; set; }
        public string Comment { get; set; } = "";
        public DateTime CreatedAt { get; set; }
    }
}
