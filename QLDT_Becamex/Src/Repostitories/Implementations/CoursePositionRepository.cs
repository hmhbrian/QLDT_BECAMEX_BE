using QLDT_Becamex.Src.Config;
using QLDT_Becamex.Src.Models;
using QLDT_Becamex.Src.Repostitories.GenericRepository;
using QLDT_Becamex.Src.Repostitories.Interfaces;

namespace QLDT_Becamex.Src.Repostitories.Implementations
{
    public class CoursePositionRepository : GenericRepository<CoursePosition>, ICoursePositionRepository
    {
        public CoursePositionRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
