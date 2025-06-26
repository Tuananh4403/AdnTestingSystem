using AdnTestingSystem.Repositories.Data;
using AdnTestingSystem.Repositories.Models;
using AdnTestingSystem.Repositories.Repositories.Generic;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdnTestingSystem.Repositories.Repositories.Repository
{
    public class BookingRepository : GenericRepository<Booking>, IBookingRepository
    {
        private readonly AdnTestingDbContext _context;

        public BookingRepository(AdnTestingDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<(List<Booking> Items, int TotalCount)> GetBookingListForStaffAsync(
            int page,
            int pageSize,
            string? searchTerm,
            BookingStatus? status)
        {
            var query = _context.Bookings
                .Include(b => b.Customer)
                    .ThenInclude(c => c.Profile)
                .Include(b => b.DnaTestService)
                    .ThenInclude(s => s.Prices)
                .Include(b => b.Transaction)
                .Include(b => b.Samples)
                .Include(b => b.TestResult)
                .Include(b => b.Rating)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var search = searchTerm.Trim().ToLower();
                query = query.Where(b =>
                    b.Id.ToString().Contains(search) ||
                    b.CustomerId.ToString().Contains(search) ||
                    (b.Customer.Profile != null &&
                     b.Customer.Profile.FullName.ToLower().Contains(search)) ||
                    b.Customer.Email.ToLower().Contains(search)
                );
            }

            if (status.HasValue)
            {
                query = query.Where(b => b.Status == status.Value);
            }

            var totalCount = await query.CountAsync();

            query = query.OrderBy(b => b.ApprovedAt == null ? 0 : 1)
                         .ThenByDescending(b => b.BookingDate);

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }
    }

    public class BookingStaffDto
    {
        public int Id { get; set; }
        public string CustomerName { get; set; } = null!;
        public string CustomerEmail { get; set; } = null!;
        public BookingStatus Status { get; set; }
        public DateTime BookingDate { get; set; }
        public string ServiceName { get; set; } = null!;
        public decimal TotalPrice { get; set; }
    }

}