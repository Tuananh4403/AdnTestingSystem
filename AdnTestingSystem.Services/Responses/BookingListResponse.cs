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

        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }

        public List<Booking> Items { get; set; } = new();
    }

}
