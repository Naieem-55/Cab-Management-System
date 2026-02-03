using System.Linq.Expressions;
using Cab_Management_System.Data;
using Microsoft.EntityFrameworkCore;

namespace Cab_Management_System.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly ApplicationDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public Repository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
            => await _dbSet.ToListAsync();

        public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
            => await _dbSet.Where(predicate).ToListAsync();

        public virtual async Task<T?> GetByIdAsync(int id)
            => await _dbSet.FindAsync(id);

        public virtual async Task AddAsync(T entity)
            => await _dbSet.AddAsync(entity);

        public virtual void Update(T entity)
            => _dbSet.Update(entity);

        public virtual void Remove(T entity)
            => _dbSet.Remove(entity);

        public virtual async Task<int> CountAsync()
            => await _dbSet.CountAsync();

        public virtual async Task<int> CountAsync(Expression<Func<T, bool>> predicate)
            => await _dbSet.CountAsync(predicate);

        public async Task SaveChangesAsync()
            => await _context.SaveChangesAsync();
    }
}
