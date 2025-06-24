using QLDT_Becamex.Src.Domain.Interfaces;
using QLDT_Becamex.Src.Domain.Models;

namespace QLDT_Becamex.Src.Infrastructure.Persistence.Repostitories
{
    public class CoursePositionRepository : GenericRepository<CoursePosition>, ICoursePositionRepository
    {
        public CoursePositionRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
