using QLDT_Becamex.Src.Domain.Interfaces;
using QLDT_Becamex.Src.Domain.Entities;

namespace QLDT_Becamex.Src.Infrastructure.Persistence.Repostitories
{
    public class UserCourseRepository : GenericRepository<UserCourse>, IUserCourseRepository
    {
        public UserCourseRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}