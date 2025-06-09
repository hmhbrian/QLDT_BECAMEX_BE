using QLDT_Becamex.Src.Models;

namespace QLDT_Becamex.Src.Repostitories.Interfaces
{
    public interface IDepartmentRepository
    {
        Task<Department?> GetByIdAsync(string id);
        Task<bool> AnyAsync(Func<Department, bool> predicate);
        Task<List<Department>> GetAllAsync();
        Task AddAsync(Department entity);
    }
}
