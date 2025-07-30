using AdnTestingSystem.Repositories.Data;
using AdnTestingSystem.Repositories.Models;
using AdnTestingSystem.Services.Interfaces;
using AdnTestingSystem.Services.Requests;
using AdnTestingSystem.Services.Responses;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdnTestingSystem.Services.Services
{
    public class RatingService : IRatingService
    {
        private readonly AdnTestingDbContext _context;

        public RatingService(AdnTestingDbContext context)
        {
            _context = context;
        }

        public async Task<CommonResponse<string>> CreateRatingAsync(int userId, NewRatingRequest request)
        {
            var booking = await _context.Bookings
                .Include(b => b.Rating)
                .FirstOrDefaultAsync(b => b.Id == request.BookingId && b.CustomerId == userId);

            if (booking == null)
                return CommonResponse<string>.Fail("Booking not found or you are not authorized.");

            if (request.Stars < 1 || request.Stars > 5)
                return CommonResponse<string>.Fail("Rating stars must be from 1 to 5.");

            if (booking.Rating != null)
            {
                booking.Rating.Stars = request.Stars;
                booking.Rating.Comment = request.Comment;
                booking.Rating.CreatedBy = request.UserId;
                booking.Rating.CreatedAt = DateTime.UtcNow;
            }
            else
            {
                var rating = new Rating
                {
                    BookingId = request.BookingId,
                    Stars = request.Stars,
                    Comment = request.Comment,
                    CreatedBy = request.UserId,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Ratings.Add(rating);
            }

            await _context.SaveChangesAsync();

            return CommonResponse<string>.Ok("OK", "Rating saved successfully.");
        }

        public async Task<CommonResponse<RatingViewModel>> GetRatingByBookingIdAsync(int bookingId)
        {
            var rating = await _context.Ratings
                .Include(r => r.Booking)
                    .ThenInclude(b => b.Customer)
                       .ThenInclude(c => c.Profile)
                .Include(r => r.Booking)
                    .ThenInclude(b => b.DnaTestService)
                .Where(r => r.BookingId == bookingId)
                .Select(r => new RatingViewModel
                {
                    Id = r.Id,
                    CustomerName = r.Booking.Customer.Profile != null
    ? r.Booking.Customer.Profile.FullName
    : "N/A",
                    ServiceName = r.Booking.DnaTestService.Name,
                    Stars = r.Stars,
                    Comment = r.Comment,
                    CreatedAt = r.CreatedAt
                })
                .FirstOrDefaultAsync();

            if (rating == null)
            {
                return CommonResponse<RatingViewModel>.Fail("No rating found for this booking.");
            }

            return CommonResponse<RatingViewModel>.Ok(rating);
        }
    }

}
