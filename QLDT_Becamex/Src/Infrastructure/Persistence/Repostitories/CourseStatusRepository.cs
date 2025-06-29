using QLDT_Becamex.Src.Domain.Interfaces;
using QLDT_Becamex.Src.Domain.Entities;


namespace QLDT_Becamex.Src.Infrastructure.Persistence.Repostitories
{
    public class CourseStatusRepository : GenericRepository<CourseStatus>, ICourseStatusRepository
    {
        public CourseStatusRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
