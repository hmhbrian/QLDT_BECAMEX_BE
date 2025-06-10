using Microsoft.EntityFrameworkCore;
using QLDT_Becamex.Src.Config;
using QLDT_Becamex.Src.Models;
using QLDT_Becamex.Src.Repostitories.GenericRepository;
using QLDT_Becamex.Src.Repostitories.Interfaces;

namespace QLDT_Becamex.Src.Repostitories.Implementations
{
    public class DepartmentRepository : GenericRepository<Department>, IDepartmentRepository
    {

        public DepartmentRepository(ApplicationDbContext dbContext) : base(dbContext)
        {

        }

        //public async Task<Department?> GetByIdAsync(string id)
        //{
        //    return await _dbContext.Departments.FindAsync(id);
        //}

        public async Task<bool> AnyAsync(Func<Department, bool> predicate)
        {
            return await Task.FromResult(_dbContext.Departments.Any(predicate));
        }

        //public async Task<List<Department>> GetAllAsync()
        //{
        //    return await _dbContext.Departments.ToListAsync();
        //}

        public async Task AddAsync(Department entity)
        {
            await _dbContext.Departments.AddAsync(entity);
        }
    }
}
