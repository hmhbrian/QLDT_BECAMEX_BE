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
        public ICourseAttachedFileRepository CourseAttachedFileRepository { get; }
        public ILessonRepository LessonRepository { get; }
        public IQuestionRepository QuestionRepository { get; }
        public IFeedbackRepository FeedbackRepository { get; }
        public ILessonProgressRepository LessonProgressRepository { get; }
        public ITypeDocumentRepository TypeDocumentRepository { get; }
        public IDepartmentStatusRepository DepartmentStatusRepository { get; }

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
            CourseAttachedFileRepository = new CourseAttachedFileRepository(dbContext);
            LessonRepository = new LessonRepository(dbContext);
            QuestionRepository = new QuestionRepository(dbContext);
            FeedbackRepository = new FeedbackRepository(dbContext);
            LessonProgressRepository = new LessonProgressRepository(dbContext);
            TypeDocumentRepository = new TypeDocumentRepository(dbContext);
            DepartmentStatusRepository = new DepartmentStatusRepository(dbContext);
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
