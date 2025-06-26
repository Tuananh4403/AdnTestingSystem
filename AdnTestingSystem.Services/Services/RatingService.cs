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

            if (booking.Rating != null)
                return CommonResponse<string>.Fail("You have already rated this booking.");

            if (request.Stars < 1 || request.Stars > 5)
                return CommonResponse<string>.Fail("Rating stars must be from 1 to 5.");

            var rating = new Rating
            {
                BookingId = request.BookingId,
                Stars = request.Stars,
                Comment = request.Comment,
                CreatedAt = DateTime.UtcNow
            };

            _context.Ratings.Add(rating);
            await _context.SaveChangesAsync();

            return CommonResponse<string>.Ok("OK", "Rating created successfully.");
        }
    }

}
