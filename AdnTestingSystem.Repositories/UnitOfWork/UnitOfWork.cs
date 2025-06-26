using AdnTestingSystem.Repositories.Data;
using AdnTestingSystem.Repositories.Models;
using AdnTestingSystem.Repositories.Repositories.Generic;

namespace AdnTestingSystem.Repositories.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AdnTestingDbContext _context;

        public IGenericRepository<User> Users { get; }
        public IGenericRepository<Booking> Bookings { get; }
        public IGenericRepository<Blog> Blogs { get; }
        public IGenericRepository<DnaTestService> DnaTestServices { get; }
        public IGenericRepository<Rating> Ratings { get; }
        public IGenericRepository<ResultTimeOption> ResultTimeOptions { get; }
        public IGenericRepository<Sample> Samples { get; }
        public IGenericRepository<ServicePrice> ServicePrices { get; }
        public IGenericRepository<TestResult> TestResults { get; }
        public IGenericRepository<Transaction> Transactions { get; }
        public IGenericRepository<UserProfile> UserProfiles { get; }

        public UnitOfWork(AdnTestingDbContext context)
        {
            _context = context;
            Users = new GenericRepository<User>(_context);
            Bookings = new GenericRepository<Booking>(_context);
            Blogs = new GenericRepository<Blog>(_context);
            DnaTestServices = new GenericRepository<DnaTestService>(_context);
            Ratings = new GenericRepository<Rating>(_context);
            ResultTimeOptions = new GenericRepository<ResultTimeOption>(_context);
            Samples = new GenericRepository<Sample>(_context);
            ServicePrices = new GenericRepository<ServicePrice>(_context);
            TestResults = new GenericRepository<TestResult>(_context);
            Transactions = new GenericRepository<Transaction>(_context);
            UserProfiles = new GenericRepository<UserProfile>(_context);
        }

        public async Task<int> CompleteAsync() => await _context.SaveChangesAsync();
    }

}
