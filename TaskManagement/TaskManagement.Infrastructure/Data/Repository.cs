namespace TaskManagement.Infrastructure.Data
{
    using Microsoft.EntityFrameworkCore;
    using TaskManagement.Application.Interfaces;

    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly TaskDbContext _context;
        public Repository(TaskDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(T entity) => await _context.Set<T>().AddAsync(entity);

        public async Task DeleteAsync(T entity) => _context.Set<T>().Remove(entity);

        public async Task DeleteRangeAsync(IEnumerable<T> entities) => _context.Set<T>().RemoveRange(entities);

        public async Task<IEnumerable<T>> GetAllAsync() => await _context.Set<T>().ToListAsync();

        public IQueryable<T> GetAll() => _context.Set<T>().AsQueryable();

        public async Task<T> GetByIdAsync(int id) => await _context.Set<T>().FindAsync(id);

        public async Task UpdateAsync(T entity) => _context.Set<T>().Update(entity);
    }
}