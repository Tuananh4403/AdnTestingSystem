

using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace AdnTestingSystem.Core.UOW
{
    public class GenericRepository<T, TContext> : IGenericRepository<T, TContext>
     where T : class
     where TContext : DbContext
    {
        protected readonly TContext _context;
        protected readonly DbSet<T> _dbSet;

        public GenericRepository(TContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public IQueryable<T> Entities => _dbSet;

        public void Delete(object id)
        {
            T entity = _dbSet.Find(id)!;
            _dbSet.Remove(entity);
        }

        public async Task DeleteAsync(object id)
        {
            T? entity = await _dbSet.FindAsync(id);
            _dbSet.Remove(entity!);
        }

        public IQueryable<T> Find(Expression<Func<T, bool>> predicate)
        {
            return  _dbSet.Where(predicate);
        }

        public IEnumerable<T> GetAll()
        {
            return _dbSet.AsEnumerable();
        }
        public async Task<IList<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }
        /// <summary>
        /// Fillter
        /// </summary>
        /// <param name="predicate"> muon filter truong nao </param>
        /// <returns></returns>
        public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> predicate)
        {
            if (predicate == null) 
            {
                return await _dbSet.ToListAsync();
            }
            return await _dbSet.Where(predicate).ToListAsync();
        }
        public T? GetById(object id)
        {
            return _dbSet.Find(id);
        }
        public async Task<T?> GetByIdAsync(object id)
        {
            return await _dbSet.FindAsync(id);
        }
        //public async Task<PaginatedList<T>> GetPagging(IQueryable<T> query, int index, int pageSize)
        //{
        //    return await query.GetPaginatedList(index, pageSize);
        //}
        public void Insert(T obj)
        {
            _dbSet.Add(obj);
        }
        public async Task InsertAsync(T obj)
        {
            await _dbSet.AddAsync(obj);
        }
        public void InsertRange(IList<T> obj)
        {
            _dbSet.AddRange(obj);
        }
        public async Task InsertRangeAsync(IList<T> obj)
        {
            await _dbSet.AddRangeAsync(obj);
        }
        public void Save()
        {
            _context.SaveChanges();
        }
        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
        public void Update(T obj)
        {
            _dbSet.Entry(obj).State = EntityState.Modified;
        }

        public Task UpdateAsync(T obj)
        {
            return Task.FromResult(_dbSet.Update(obj));
        }


    }
 }
