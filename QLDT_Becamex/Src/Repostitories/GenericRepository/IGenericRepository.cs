using System.Linq.Expressions;

namespace QLDT_Becamex.Src.Repostitories.GenericRepository
{
    public interface IGenericRepository<T> where T : class
    {
        // Lấy tất cả các thực thể
        public Task<IEnumerable<T>> GetAllAsync();

        // Lấy thực thể theo ID
        public Task<T?> GetByIdAsync(string id); // Sử dụng string cho ID nếu các model của bạn dùng string ID

        // Tìm kiếm các thực thể dựa trên một điều kiện
        public Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);

        // Thêm một thực thể mới
        public Task AddAsync(T entity);

        // Thêm nhiều thực thể mới
        public Task AddRangeAsync(IEnumerable<T> entities);

        // Cập nhật một thực thể
        public void Update(T entity); // Update thường không cần async vì EF Core theo dõi trạng thái

        // Xóa một thực thể
        public void Remove(T entity);

        // Xóa nhiều thực thể
        public void RemoveRange(IEnumerable<T> entities);

        public Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);
    }
}
