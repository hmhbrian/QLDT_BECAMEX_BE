using QLDT_Becamex.Src.Domain.Interfaces;
using QLDT_Becamex.Src.Domain.Entities;

namespace QLDT_Becamex.Src.Infrastructure.Persistence.Repostitories
{
    public class CourseELevelRepository : GenericRepository<CourseELevel>, ICourseElevelRepository
    {
        public CourseELevelRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
