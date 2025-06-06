using Microsoft.EntityFrameworkCore;
namespace AdnTestingSystem.Core.UOW
{
    public class UnitOfWork<T ,TContext> : IUnitOfWork<T, TContext> where TContext : DbContext  where T : class
    {
        private bool disposed = false;
        private readonly TContext _dbContext;
        private readonly Dictionary<Type, object> _repositories;
        public UnitOfWork(TContext dbContext)
        {
            _dbContext = dbContext;
            _repositories = [];
        }
        public void BeginTransaction()
        {
            _dbContext.Database.BeginTransaction();
        }

        public void CommitTransaction()
        {
            _dbContext.Database.CommitTransaction();
        }
        public void Dispose()
        {
            // tự huỷ khi xài xong 
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    _dbContext.Dispose();
                }
            }
            disposed = true;
        }

        public void RollBack()
        {
            _dbContext.Database.RollbackTransaction();
        }

        public void Save()
        {
            _dbContext.SaveChanges();
        }
        public async Task SaveAsync()
        {
            await _dbContext.SaveChangesAsync();
        }

        public IGenericRepository< T,TContext > GetRepository<T>() where T : class
        {
            if (_repositories.ContainsKey(typeof(T)))
            {
                return (IGenericRepository<T,TContext>)_repositories[typeof(T)];
            }

            GenericRepository<T,TContext> repository = new(_dbContext);
            _repositories.Add(typeof(T), repository);
            return repository;
        }
    }
}
