using Microsoft.EntityFrameworkCore;
using QLDT_Becamex.Src.Domain.Entities;
using QLDT_Becamex.Src.Domain.Interfaces;

namespace QLDT_Becamex.Src.Infrastructure.Persistence.Repostitories
{
    public class TestRepository : GenericRepository<Test>, ITestRepository
    {
        private readonly ApplicationDbContext _context;
        public TestRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _context = dbContext;
        }

        public async Task UpdatePositionAsync(string courseId, int fromPosition, int toPosition, int offset)
        {
            await _context.Tests
                .Where(l => l.CourseId == courseId && l.Position >= fromPosition && l.Position <= toPosition)
                .ExecuteUpdateAsync(l => l.SetProperty(x => x.Position, x => x.Position + offset));
        }
    }
}
