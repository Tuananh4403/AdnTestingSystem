using AdnTestingSystem.Repositories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AdnTestingSystem.Repositories.Repositories.Repository.BookingRepository;

namespace AdnTestingSystem.Services.Responses
{
    public class BookingListResponse
    {

        public int Id { get; set; }
        public string Code => $"ĐH{Id}";
        public string ServiceName { get; set; } = string.Empty;
        public int SampleMethod { get; set; }
        public int ResultTime { get; set; }
        public decimal TotalPrice { get; set; }
        public string? SampleDate { get; set; }
        public string BookingDate { get; set; } = string.Empty;
        public int Status { get; set; }
        public bool IsCivil { get; set; }
    }

}
