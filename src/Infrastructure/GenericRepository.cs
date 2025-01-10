using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Infrastructure
{
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity, IAggregateRoot
    {
        protected readonly ApplicationDbContext _context;
        internal DbSet<T> _set;

        //Impliment when diff repostiryes are used on the same tracnaction
        //public IUnitOfWork UnitOfWork => _context;

        public GenericRepository(ApplicationDbContext context)
        {
            _context = context;
            _set = _context.Set<T>();
        }
        public IEnumerable<T> Specify(ISpecification<T> spec)
        {
            var includes = spec.Includes.Aggregate(_context.Set<T>().AsQueryable(),
 (current, include) => current.Include(include));

            return includes
            .Where(spec.Criteria)
            .AsEnumerable();
        }

        //can be optional
        public IEnumerable<T> SpecifyWithPagination(ISpecification<T> spec, int pageSize = 10, int pageIndex = 0)
        {
            var includes = spec.Includes.Aggregate(_context.Set<T>().AsQueryable(), (current, include) => current.Include(include));

            return includes
            .Where(spec.Criteria).Skip(pageSize * pageIndex).Take(pageSize)
            .AsEnumerable();
        }

        public async Task<T> AddAsync(T entity)
        {
            await _set.AddAsync(entity);
            return entity;
        }
        public async Task<T[]> AddRangeAsync(T[] entity)
        {
            await _set.AddRangeAsync(entity);
            return entity;
        }
        public Task DeleteAsync(T entity)
        {
            _set.Remove(entity);
            return Task.CompletedTask;
        }
        public async Task<T> Get(Expression<Func<T, bool>> predicate)
        {
            var data = await _set.FirstOrDefaultAsync(predicate);
            return data;
        }
        public async Task<IReadOnlyList<T>> GetAll()
        {
            var result = await _set.ToListAsync();
            return result;
        }
        public async Task<T> GetByIdAsync(int Id)
        {
            var data = await _set.FindAsync(Id);
            return data;
        }
        public Task UpdateAsync(T entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
            return Task.CompletedTask;
        }

        public Task UpdateRangeAsync(T[] entity)
        {
            Array.ForEach(entity, e => _context.Entry(e).State = EntityState.Modified);
            return Task.CompletedTask;
        }

        public async Task AddOrUpdateAsync(T entity)
        {
            var exists = await GetByIdAsync(entity.Id);

            if (exists == null)
            {
                entity.Id = default;
                await AddAsync(entity);
            }
            else
            {                
                await UpdateAsync(entity);
            }
        }

        public async Task CommitAsync(CancellationToken cancellationToken)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
