using QLDT_Becamex.Src.Repostitories.Interfaces;

namespace QLDT_Becamex.Src.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        public IUserRepository UserRepository { get; }
        public IDepartmentRepository DepartmentRepository { get; }
        public IPositionRepostiory PositionRepostiory { get; }
        public IUserStatusRepostiory UserStatusRepostiory { get; }
        public ICourseDepartmentRepository CourseDepartmentRepository { get; }
        public ICoursePositionRepository CoursePositionRepository { get; }
        public ICourseStatusRepository CourseStatusRepository { get; }
        public ICourseRepository CourseRepository { get; }
        public IUserCourseRepository UserCourseRepository { get; }



        public Task<int> CompleteAsync();
    }
}
