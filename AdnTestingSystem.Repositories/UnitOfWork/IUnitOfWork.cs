using AdnTestingSystem.Repositories.Models;
using AdnTestingSystem.Repositories.Repositories.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdnTestingSystem.Repositories.UnitOfWork
{
    public interface IUnitOfWork
    {
        IGenericRepository<User> Users { get; }
        IGenericRepository<Booking> Bookings { get; }
        IGenericRepository<DnaTestService> DnaTestServices { get; }
        IGenericRepository<Rating> Ratings { get; }
        IGenericRepository<ResultTimeOption> ResultTimeOptions { get; }
        IGenericRepository<Sample> Samples { get; }
        IGenericRepository<ServicePrice> ServicePrices { get; }
        IGenericRepository<TestResult> TestResults { get; }
        IGenericRepository<Transaction> Transactions { get; }
        IGenericRepository<UserProfile> UserProfiles { get; }
        IGenericRepository<SampleReceipt> SampleReceipts { get; }
        IGenericRepository<SampleReceiptDetail> SampleReceiptDetails { get; }
        IGenericRepository<TestResultDetail> TestResultDetails { get; }

        Task<int> CompleteAsync();
    }

}
