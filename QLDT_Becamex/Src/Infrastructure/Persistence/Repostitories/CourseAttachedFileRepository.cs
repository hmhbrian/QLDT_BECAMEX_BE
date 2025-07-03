using QLDT_Becamex.Src.Domain.Entities;
using QLDT_Becamex.Src.Domain.Interfaces;

namespace QLDT_Becamex.Src.Infrastructure.Persistence.Repostitories
{
    public class CourseAttachedFileRepository : GenericRepository<CourseAttachedFile>, ICourseAttachedFileRepository
    {
        public CourseAttachedFileRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
