using Microsoft.EntityFrameworkCore;
using QLDT_Becamex.Src.Config;
using QLDT_Becamex.Src.Models;
using QLDT_Becamex.Src.Repostitories.Interfaces;

namespace QLDT_Becamex.Src.Repostitories.Implementations
{
    public class DepartmentRepository : IDepartmentRepository
    {
        private readonly ApplicationDbContext _dbContext;
        public DepartmentRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Department?> GetByIdAsync(string id)
        {
            return await _dbContext.Set<Department>().FindAsync(id);
        }

        public async Task<bool> AnyAsync(Func<Department, bool> predicate)
        {
            return await Task.FromResult(_dbContext.Set<Department>().Any(predicate));
        }

        public async Task<List<Department>> GetAllAsync()
        {
            return await _dbContext.Set<Department>().ToListAsync();
        }

        public async Task AddAsync(Department entity)
        {
            await _dbContext.Set<Department>().AddAsync(entity);
        }
    }
}
