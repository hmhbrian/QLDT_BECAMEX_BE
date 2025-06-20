using QLDT_Becamex.Src.Config;
using QLDT_Becamex.Src.Repostitories.Implementations;
using QLDT_Becamex.Src.Repostitories.Interfaces;

namespace QLDT_Becamex.Src.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _dbContext;
        public IUserRepository UserRepository { get; }
        public IDepartmentRepository DepartmentRepository { get; }
        public IPositionRepostiory PositionRepostiory { get; }
        public IUserStatusRepostiory UserStatusRepostiory { get; }

        public ICourseDepartmentRepository CourseDepartmentRepository { get; }
        public ICoursePostitionRepository CoursePostitionRepository { get; }
        public ICourseStatusRepository CourseStatusRepository { get; }

        public UnitOfWork(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            UserRepository = new UserRepository(dbContext);
            DepartmentRepository = new DepartmentRepository(dbContext);
            PositionRepostiory = new PositionRepository(dbContext);
            UserStatusRepostiory = new UserStatusRepostiory(dbContext);
            CourseDepartmentRepository = new CourseDepartmentRepository(dbContext);
            CoursePostitionRepository = new CoursePostitionRepository(dbContext);
            CourseStatusRepository = new CourseStatusRepository(dbContext);
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
