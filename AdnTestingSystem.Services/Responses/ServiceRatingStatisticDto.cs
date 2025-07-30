using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdnTestingSystem.Services.Responses
{
    public class ServiceRatingStatisticDto
    {
        public int ServiceId { get; set; }
        public string ServiceName { get; set; } = "";
        public int TotalRatings { get; set; }
        public int TotalStars { get; set; }
        public double AverageStars { get; set; }
        public double RatingPercentage { get; set; }
    }
}