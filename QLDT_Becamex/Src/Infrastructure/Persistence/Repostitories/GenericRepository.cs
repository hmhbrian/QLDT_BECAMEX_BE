using Microsoft.EntityFrameworkCore;
using QLDT_Becamex.Src.Domain.Interfaces;
using System.Linq.Expressions;

namespace QLDT_Becamex.Src.Infrastructure.Persistence.Repostitories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly ApplicationDbContext _dbContext;
        private readonly DbSet<T> _dbSet;
        public GenericRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = dbContext.Set<T>();
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {

            return await _dbContext.Set<T>().ToListAsync();
        }

        public async Task<T?> GetByIdAsync(object id)
        {
            return await _dbContext.Set<T>().FindAsync(new object[] { id });
        }

        public async Task<T?> GetFirstOrDefaultAsync(
        Expression<Func<T, bool>> predicate,
        Func<IQueryable<T>, IQueryable<T>>? includes = null)
        {
            IQueryable<T> query = _dbContext.Set<T>();

            if (includes != null)
            {
                query = includes(query); // Apply includes nếu có
            }

            return await query.FirstOrDefaultAsync(predicate);
        }
        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {

            return await _dbContext.Set<T>().Where(predicate).ToListAsync();
        }
        public async Task<List<TResult>> FindAndSelectAsync<TResult>(
            Expression<Func<T, bool>> predicate,
            Expression<Func<T, TResult>> selector)
        {
            return await _dbContext.Set<T>()
                .Where(predicate)
                .Select(selector)
                .ToListAsync();
        }

        public async Task AddAsync(T entity)
        {

            await _dbContext.Set<T>().AddAsync(entity);
        }

        public async Task AddRangeAsync(IEnumerable<T> entities)
        {

            await _dbContext.Set<T>().AddRangeAsync(entities);
        }

        public void UpdateEntity(T entity)
        {

            _dbContext.Set<T>().Attach(entity);
            _dbContext.Entry(entity).State = EntityState.Modified;
        }

        public void Update(T entity, T updatedEntity)
        {
            var entry = _dbContext.Entry(entity);

            foreach (var prop in entry.Properties)
            {
                var propertyInfo = typeof(T).GetProperty(prop.Metadata.Name);
                if (propertyInfo == null) continue;

                var updatedValue = propertyInfo.GetValue(updatedEntity);
                var currentValue = prop.CurrentValue;

                if (!Equals(currentValue, updatedValue))
                {
                    prop.CurrentValue = updatedValue;
                    prop.IsModified = true;
                }
            }
        }

        public void Remove(T entity)
        {

            _dbContext.Set<T>().Remove(entity);
        }

        public void RemoveRange(IEnumerable<T> entities)
        {

            _dbContext.Set<T>().RemoveRange(entities);
        }

        //Custom thêm

        public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbContext.Set<T>().AnyAsync(predicate);
        }


        // Lấy tổng items của 1 bảng 
        public async Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null)
        {
            IQueryable<T> query = _dbContext.Set<T>();
            if (predicate != null)
            {
                query = query.Where(predicate);
            }
            return await query.CountAsync();
        }

        //Get all linh hoạt
        public async Task<IEnumerable<T>> GetFlexibleAsync(
        Expression<Func<T, bool>>? predicate = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        int? page = null,
        int? pageSize = null,
        bool asNoTracking = false,
        Func<IQueryable<T>, IQueryable<T>>? includes = null)
        {
            IQueryable<T> query = _dbContext.Set<T>();

            if (asNoTracking)
            {
                query = query.AsNoTracking();
            }

            if (includes != null)
            {
                query = includes(query);
            }

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            int totalCount = await query.CountAsync();

            if (orderBy != null)
            {
                query = orderBy(query);
            }

            if (page.HasValue && pageSize.HasValue)
            {
                int skip = (page.Value - 1) * pageSize.Value;
                query = query.Skip(skip).Take(pageSize.Value);
            }

            return await query.ToListAsync();
        }

        public IQueryable<T> GetQueryable()
        {
            return _dbSet.AsQueryable();
        }
    }
}
