using Microsoft.EntityFrameworkCore;
using QLDT_Becamex.Src.Domain.Entities;
using QLDT_Becamex.Src.Domain.Interfaces;

namespace QLDT_Becamex.Src.Infrastructure.Persistence.Repostitories
{
    public class LessonRepository : GenericRepository<Lesson>, ILessonRepository
    {
        private readonly ApplicationDbContext _context;
        public LessonRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _context = dbContext;
        }
        public async Task<Lesson?> GetByIdAsync(int id)
        {
            return await _context.Lessons
                .Include(l => l.CreatedBy)
                .Include(l => l.UpdatedBy)
                .FirstOrDefaultAsync(l => l.Id == id);
        }
        public async Task<int> GetMaxPositionAsync(string courseId)
        {
            return await _context.Lessons
                .Where(l => l.CourseId == courseId)
                .MaxAsync(l => (int?)l.Position) ?? 0;
        }

        public async Task UpdatePositionAsync(string courseId, int fromPosition, int toPosition, int offset)
        {
            await _context.Lessons
                .Where(l => l.CourseId == courseId && l.Position >= fromPosition && l.Position <= toPosition)
                .ExecuteUpdateAsync(l => l.SetProperty(x => x.Position, x => x.Position + offset));
        }

        public async Task ReorderPositionsAsync(string courseId, CancellationToken cancellationToken = default)
        {
            // Lấy tất cả Lesson còn lại trong Course, sắp xếp theo Position
            var lessons = await _context.Lessons
                .Where(l => l.CourseId == courseId)
                .OrderBy(l => l.Position)
                .ToListAsync(cancellationToken);

            // Gán lại Position từ 1
            for (int i = 0; i < lessons.Count; i++)
            {
                lessons[i].Position = i + 1;
            }
        }
    }
}
