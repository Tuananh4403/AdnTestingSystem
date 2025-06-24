using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdnTestingSystem.Repositories.AdnTestingSystem.Repositories.Implementations;
using AdnTestingSystem.Repositories.Models;

namespace AdnTestingSystem.Services
{

        public class BookingService
        {
            private readonly BookingRepository _bookingRepository;

            public BookingService(BookingRepository bookingRepository)
            {
                _bookingRepository = bookingRepository;
            }

            public async Task<int> CreateBookingAsync(int customerId, int serviceId, SampleMethod sampleMethod)
            {
                var newBooking = new Booking
                {
                    CustomerId = customerId,
                    DnaTestServiceId = serviceId,
                    SampleMethod = sampleMethod,
                    BookingDate = DateTime.UtcNow,
                    Status = BookingStatus.Pending
                };

                return await _bookingRepository.CreateBookingAsync(newBooking);
            }
        }
  }


