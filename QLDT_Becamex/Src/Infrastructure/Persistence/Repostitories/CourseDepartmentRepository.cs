using QLDT_Becamex.Src.Domain.Interfaces;
using QLDT_Becamex.Src.Domain.Entities;

namespace QLDT_Becamex.Src.Infrastructure.Persistence.Repostitories
{
    public class CourseDepartmentRepository : GenericRepository<CourseDepartment>, ICourseDepartmentRepository
    {
        public CourseDepartmentRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
