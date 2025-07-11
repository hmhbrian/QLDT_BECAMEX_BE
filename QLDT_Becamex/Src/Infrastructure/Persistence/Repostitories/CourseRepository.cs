using QLDT_Becamex.Src.Domain.Interfaces;
using QLDT_Becamex.Src.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace QLDT_Becamex.Src.Infrastructure.Persistence.Repostitories
{
    public class CourseRepository : GenericRepository<Course>, ICourseRepository
    {
        private readonly ApplicationDbContext _context;
        public CourseRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _context = dbContext;
        }

        public async Task<List<Course>> GetCoursesForStatusUpdateAsync(DateTime currentDate)
        {
            return await _context.Course
            .Where(c => c.StatusId != 4 &&
                       (c.RegistrationStartDate <= currentDate.AddDays(1) ||
                        c.RegistrationClosingDate <= currentDate.AddDays(1) ||
                        c.StartDate <= currentDate.AddDays(1) ||
                        c.EndDate <= currentDate.AddDays(1)))
            .ToListAsync();
        }
    }
}
