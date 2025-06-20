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
        public ICoursePostitionRepository CoursePostitionRepository { get; }
        public ICourseStatusRepository CourseStatusRepository { get; }



        public Task<int> CompleteAsync();
    }
}
