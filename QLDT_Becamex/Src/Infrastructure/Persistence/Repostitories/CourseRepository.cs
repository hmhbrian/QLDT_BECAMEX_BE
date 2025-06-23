using QLDT_Becamex.Src.Domain.Interfaces;
using QLDT_Becamex.Src.Domain.Models;

namespace QLDT_Becamex.Src.Infrastructure.Persistence.Repostitories
{
    public class CourseRepository : GenericRepository<Course>, ICourseRepository
    {
        public CourseRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
