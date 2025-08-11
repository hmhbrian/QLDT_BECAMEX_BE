using QLDT_Becamex.Src.Infrastructure.Persistence.Repostitories;
using System.Data.Common;

namespace QLDT_Becamex.Src.Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        public IUserRepository UserRepository { get; }
        public IDepartmentRepository DepartmentRepository { get; }
        public IEmployeeLevelRepository EmployeeLevelRepository { get; }
        public IUserStatusRepostiory UserStatusRepository { get; }
        public ICourseDepartmentRepository CourseDepartmentRepository { get; }
        public ICourseElevelRepository CourseELevelRepository { get; }
        public ICourseStatusRepository CourseStatusRepository { get; }
        public ICourseRepository CourseRepository { get; }
        public IUserCourseRepository UserCourseRepository { get; }
        public ICourseCategoryRepository CourseCategoryRepository { get; }
        public ITestRepository TestRepository { get; }
        public ICourseAttachedFileRepository CourseAttachedFileRepository { get; }
        public ILessonRepository LessonRepository { get; }
        public IQuestionRepository QuestionRepository { get; }
        public IFeedbackRepository FeedbackRepository { get; }
        public ILessonProgressRepository LessonProgressRepository { get; }
        public ITypeDocumentRepository TypeDocumentRepository { get; }
        public IDepartmentStatusRepository DepartmentStatusRepository { get; }
        public ITestResultRepository TestResultRepository { get; }
        public IUserAnswerRepository UserAnswerRepository { get; }
        public IAuditLogRepository AuditLogRepository { get; }
        public ICertificatesRepository CertificatesRepository { get; }

        public Task<int> CompleteAsync();
        Task<DbTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);
    }
}
