using Microsoft.EntityFrameworkCore;
using QLDT_Becamex.Src.Config;
using QLDT_Becamex.Src.Dtos.Results;
using System.Linq.Expressions;

namespace QLDT_Becamex.Src.Repostitories.GenericRepository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly ApplicationDbContext _dbContext;
        public GenericRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {

            return await _dbContext.Set<T>().ToListAsync();
        }

        public async Task<T?> GetByIdAsync(object id)
        {
            return await _dbContext.Set<T>().FindAsync(new object[] { id });
        }

        public async Task<T?> GetFirstOrDefaultAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbContext.Set<T>().Where(predicate).FirstOrDefaultAsync();
        }
        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {

            return await _dbContext.Set<T>().Where(predicate).ToListAsync();
        }


        public async Task AddAsync(T entity)
        {

            await _dbContext.Set<T>().AddAsync(entity);
        }

        public async Task AddRangeAsync(IEnumerable<T> entities)
        {

            await _dbContext.Set<T>().AddRangeAsync(entities);
        }

        public void Update(T entity)
        {

            _dbContext.Set<T>().Attach(entity);
            _dbContext.Entry(entity).State = EntityState.Modified;
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
        params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _dbContext.Set<T>();

            // AsNoTracking nếu được bật
            if (asNoTracking)
            {
                query = query.AsNoTracking();
            }

            // Include các bảng liên quan
            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            // Lọc theo điều kiện
            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            // Lưu lại số lượng trước khi phân trang
            int totalCount = await query.CountAsync();

            // Sắp xếp
            if (orderBy != null)
            {
                query = orderBy(query);
            }

            // Phân trang
            if (page.HasValue && pageSize.HasValue)
            {
                int skip = (page.Value - 1) * pageSize.Value;
                query = query.Skip(skip).Take(pageSize.Value);
            }

            var items = await query.ToListAsync();
            return items;

        }

    }
}
