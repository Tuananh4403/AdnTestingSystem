using AdnTestingSystem.Repositories.Models;
using AdnTestingSystem.Repositories.Repositories.Generic;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AdnTestingSystem.Repositories.Repositories.Repository
{
    public interface IBookingRepository : IGenericRepository<Booking>
    {
        Task<(List<Booking> Items, int TotalCount)> GetBookingListForStaffAsync(
            int page,
            int pageSize,
            string? searchTerm,
            BookingStatus? status);
    }
}