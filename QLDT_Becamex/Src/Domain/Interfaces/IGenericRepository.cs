using QLDT_Becamex.Src.Domain.Entities;

using System.Linq.Expressions;

namespace QLDT_Becamex.Src.Domain.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        // Lấy tất cả các thực thể
        public Task<IEnumerable<T>> GetAllAsync();

        // Lấy thực thể theo ID
        Task<T?> GetByIdAsync(object id); // Có thể dùng string, int khi tìm kiếm theo ID

        public Task<T?> GetFirstOrDefaultAsync(Expression<Func<T, bool>> predicate, Func<IQueryable<T>, IQueryable<T>>? includes = null);
        // Tìm kiếm các thực thể dựa trên một điều kiện
        public Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);

        // Thêm một thực thể mới
        public Task AddAsync(T entity);

        // Thêm nhiều thực thể mới
        public Task AddRangeAsync(IEnumerable<T> entities);

        // Cập nhật một thực thể
        public void UpdateEntity(T entity); // Update thường không cần async vì EF Core theo dõi trạng thái
        public void Update(T entity, T updatedEntity);// Cập nhật thực thể với thực thể mới

        // Xóa một thực thể
        public void Remove(T entity);

        // Xóa nhiều thực thể
        public void RemoveRange(IEnumerable<T> entities);

        //Custom thêm
        public Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);

        public Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null);

        //Get all linh hoạt 

        public Task<IEnumerable<T>> GetFlexibleAsync(Expression<Func<T, bool>>? predicate = null,
             Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
             int? page = null,
             int? pageSize = null,
             bool asNoTracking = false,
             Func<IQueryable<T>, IQueryable<T>>? includes = null);

        // Lấy một IQueryable để thực hiện các truy vấn phức tạp hơn
        public IQueryable<T> GetQueryable();

        public Task<List<TResult>> FindAndSelectAsync<TResult>(
            Expression<Func<T, bool>> predicate,
            Expression<Func<T, TResult>> selector);
    }
}
