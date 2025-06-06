using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace AdnTestingSystem.Core.UOW
{
    public interface IGenericRepository<T, TContext>
    where T : class
    where TContext : DbContext
    {    
        IQueryable<T> Entities { get; }
        void Delete(object id);
        Task DeleteAsync(object id);
        IEnumerable<T> GetAll();
        Task<IList<T>> GetAllAsync();
        Task<IEnumerable<T>> GetAllAsync(Expression<Func< T, bool>> predicate);
        IQueryable<T> Find(Expression<Func< T, bool>> predicate);
        T? GetById(object id);
        Task<T?> GetByIdAsync(object id);
        //Task<PaginatedList<T>> GetPagging(IQueryable<T> query, int index, int pageSize);
        void Insert(T obj);
        Task InsertAsync(T obj);
        void InsertRange(IList<T> obj);
        Task InsertRangeAsync(IList<T> obj);
        void Save();
        Task SaveAsync();
        void Update(T obj);
        Task UpdateAsync(T obj);
    }
}
