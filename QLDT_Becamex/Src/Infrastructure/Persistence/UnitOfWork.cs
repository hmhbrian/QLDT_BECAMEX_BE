using QLDT_Becamex.Src.Domain.Interfaces;
using QLDT_Becamex.Src.Infrastructure.Persistence.Repostitories;


namespace QLDT_Becamex.Src.Infrastructure.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _dbContext;
        public IUserRepository UserRepository { get; }
        public IDepartmentRepository DepartmentRepository { get; }
        public IPositionRepostiory PositionRepository { get; }
        public IUserStatusRepostiory UserStatusRepository { get; }

        public ICourseDepartmentRepository CourseDepartmentRepository { get; }
        public ICoursePositionRepository CoursePositionRepository { get; }
        public ICourseStatusRepository CourseStatusRepository { get; }
        public ICourseRepository CourseRepository { get; }
        public IUserCourseRepository UserCourseRepository { get; }
        public ICourseCategoryRepository CourseCategoryRepository { get; }
        public ILecturerRepository LecturerRepository { get; }
        public ITestRepository TestRepository { get; }


        public UnitOfWork(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            UserRepository = new UserRepository(dbContext);
            DepartmentRepository = new DepartmentRepository(dbContext);
            PositionRepository = new PositionRepository(dbContext);
            UserStatusRepository = new UserStatusRepostiory(dbContext);
            CourseDepartmentRepository = new CourseDepartmentRepository(dbContext);
            CoursePositionRepository = new CoursePositionRepository(dbContext);
            CourseStatusRepository = new CourseStatusRepository(dbContext);
            CourseRepository = new CourseRepository(dbContext);
            UserCourseRepository = new UserCourseRepository(dbContext);
            LecturerRepository = new LecturerRepository(dbContext);
            CourseCategoryRepository = new CourseCategoryRepository(dbContext);
            TestRepository = new TestRepository(dbContext);
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
