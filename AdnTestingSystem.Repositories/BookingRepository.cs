using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdnTestingSystem.Repositories.Data;
using AdnTestingSystem.Repositories.Models;
using Microsoft.EntityFrameworkCore;
namespace AdnTestingSystem.Repositories
{


    namespace AdnTestingSystem.Repositories.Implementations
    {
        public class BookingRepository
        {
            private readonly AdnTestingDbContext _context;

            public BookingRepository(AdnTestingDbContext context)
            {
                _context = context;
            }

            public async Task<int> CreateBookingAsync(Booking booking)
            {
                _context.Bookings.Add(booking);
                await _context.SaveChangesAsync();
                return booking.Id;
            }
        }
    }

}
