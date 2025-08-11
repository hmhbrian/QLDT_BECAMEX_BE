using Microsoft.EntityFrameworkCore.Storage;
using QLDT_Becamex.Src.Domain.Interfaces;
using QLDT_Becamex.Src.Infrastructure.Persistence.Repostitories;
using System.Data.Common;


namespace QLDT_Becamex.Src.Infrastructure.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _dbContext;
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


        public UnitOfWork(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            UserRepository = new UserRepository(dbContext);
            DepartmentRepository = new DepartmentRepository(dbContext);
            EmployeeLevelRepository = new EmployeeLevelRepository(dbContext);
            UserStatusRepository = new UserStatusRepostiory(dbContext);
            CourseDepartmentRepository = new CourseDepartmentRepository(dbContext);
            CourseELevelRepository = new CourseELevelRepository(dbContext);
            CourseStatusRepository = new CourseStatusRepository(dbContext);
            CourseRepository = new CourseRepository(dbContext);
            UserCourseRepository = new UserCourseRepository(dbContext);
            CourseCategoryRepository = new CourseCategoryRepository(dbContext);
            TestRepository = new TestRepository(dbContext);
            CourseAttachedFileRepository = new CourseAttachedFileRepository(dbContext);
            LessonRepository = new LessonRepository(dbContext);
            QuestionRepository = new QuestionRepository(dbContext);
            FeedbackRepository = new FeedbackRepository(dbContext);
            LessonProgressRepository = new LessonProgressRepository(dbContext);
            TypeDocumentRepository = new TypeDocumentRepository(dbContext);
            DepartmentStatusRepository = new DepartmentStatusRepository(dbContext);
            TestResultRepository = new TestResultRepository(dbContext);
            UserAnswerRepository = new UserAnswerRepository(dbContext);
            AuditLogRepository = new AuditLogRepository(dbContext);
            CertificatesRepository = new CertificatesRepository(dbContext);
        }

        public async Task<int> CompleteAsync()
        {
            return await _dbContext.SaveChangesAsync();
        }

        public async Task<DbTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            var transasction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
            return transasction.GetDbTransaction();
        }
        public void Dispose()
        {
            _dbContext.Dispose();
        }
    }
}
