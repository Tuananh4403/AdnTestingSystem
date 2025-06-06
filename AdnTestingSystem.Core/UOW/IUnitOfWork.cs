using Microsoft.EntityFrameworkCore;
namespace AdnTestingSystem.Core.UOW
{
    public interface IUnitOfWork<T , TContext> : IDisposable where TContext : DbContext where T : class
    {
        void BeginTransaction();
        void CommitTransaction();
        void RollBack();
        void Save();
        Task SaveAsync();
        IGenericRepository<T, TContext> GetRepository<T>() where T : class;
    }
}
