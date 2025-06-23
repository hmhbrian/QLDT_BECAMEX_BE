using QLDT_Becamex.Src.Infrastructure;
using QLDT_Becamex.Src.Infrastructure.Persistence.Repostitories.Implementations;
using QLDT_Becamex.Src.Infrastructure.Persistence.Repostitories.Interfaces;


namespace QLDT_Becamex.Src.Infrastructure.Persistence.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _dbContext;
        public IUserRepository UserRepository { get; }
        public IDepartmentRepository DepartmentRepository { get; }
        public IPositionRepostiory PositionRepostiory { get; }
        public IUserStatusRepostiory UserStatusRepostiory { get; }

        public ICourseDepartmentRepository CourseDepartmentRepository { get; }
        public ICoursePositionRepository CoursePositionRepository { get; }
        public ICourseStatusRepository CourseStatusRepository { get; }
        public ICourseRepository CourseRepository { get; }
        public IUserCourseRepository UserCourseRepository { get; }



        public UnitOfWork(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            UserRepository = new UserRepository(dbContext);
            DepartmentRepository = new DepartmentRepository(dbContext);
            PositionRepostiory = new PositionRepository(dbContext);
            UserStatusRepostiory = new UserStatusRepostiory(dbContext);
            CourseDepartmentRepository = new CourseDepartmentRepository(dbContext);
            CoursePositionRepository = new CoursePositionRepository(dbContext);
            CourseStatusRepository = new CourseStatusRepository(dbContext);
            CourseRepository = new CourseRepository(dbContext);
            UserCourseRepository = new UserCourseRepository(dbContext);
        }

        public async Task<int> CompleteAsync()
        {
            return await _dbContext.SaveChangesAsync();
        }
        public void Dispose()
        {
            _dbContext.Dispose();
        }
    }
}
