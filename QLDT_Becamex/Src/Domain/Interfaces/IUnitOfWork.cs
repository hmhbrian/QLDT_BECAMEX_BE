namespace QLDT_Becamex.Src.Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
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
        public ICourseAttachedFileRepository CourseAttachedFileRepository { get; }




        public Task<int> CompleteAsync();
    }
}
