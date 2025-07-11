using Microsoft.EntityFrameworkCore;
using QLDT_Becamex.Src.Domain.Entities;
using QLDT_Becamex.Src.Domain.Interfaces;

namespace QLDT_Becamex.Src.Infrastructure.Persistence.Repostitories
{
    public class QuestionRepository : GenericRepository<Question>, IQuestionRepository
    {
        private readonly ApplicationDbContext _context;
        public QuestionRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _context = dbContext;
        }
        public async Task<int> GetMaxPositionAsync(int testId)
        {
            return await _context.Questions
                .Where(l => l.TestId == testId)
                .MaxAsync(l => (int?)l.Position) ?? 0;
        }
    }
}
