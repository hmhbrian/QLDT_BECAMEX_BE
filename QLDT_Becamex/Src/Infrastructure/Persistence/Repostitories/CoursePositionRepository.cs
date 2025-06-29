using QLDT_Becamex.Src.Domain.Interfaces;
using QLDT_Becamex.Src.Domain.Entities;

namespace QLDT_Becamex.Src.Infrastructure.Persistence.Repostitories
{
    public class CoursePositionRepository : GenericRepository<CoursePosition>, ICoursePositionRepository
    {
        public CoursePositionRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
