using QLDT_Becamex.Src.Domain.Entities;
using QLDT_Becamex.Src.Domain.Interfaces;

namespace QLDT_Becamex.Src.Infrastructure.Persistence.Repostitories
{
    public class ReportsRepository : IReportsRepository
    {
        private readonly ApplicationDbContext _context;
        public ReportsRepository(ApplicationDbContext dbContext)
        {
            _context = dbContext;
        }
    }
}
