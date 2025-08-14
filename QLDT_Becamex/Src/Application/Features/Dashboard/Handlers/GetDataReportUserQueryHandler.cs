using AutoMapper;
using MediatR;
using QLDT_Becamex.Src.Application.Common.Dtos;
using QLDT_Becamex.Src.Application.Features.Courses.Dtos;
using QLDT_Becamex.Src.Application.Features.Dashboard.Dtos;
using QLDT_Becamex.Src.Application.Features.Dashboard.Queries;
using QLDT_Becamex.Src.Constant;
using QLDT_Becamex.Src.Domain.Entities;
using QLDT_Becamex.Src.Domain.Interfaces;
using QLDT_Becamex.Src.Infrastructure.Persistence;
using QLDT_Becamex.Src.Infrastructure.Services;
using QLDT_Becamex.Src.Infrastructure.Services.UserServices;

namespace QLDT_Becamex.Src.Application.Features.Dashboard.Handlers
{
    public class GetDataReportUserQueryHandler : IRequestHandler<GetDashBoardUserQuery, DataReportUserDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ApplicationDbContext _context;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public GetDataReportUserQueryHandler(
            IUnitOfWork unitOfWork,
             IUserService userService,
             IMapper mapper,
            ApplicationDbContext context)
        {
            _unitOfWork = unitOfWork;
            _userService = userService;
            _context = context;
            _mapper = mapper;
        }

        public async Task<DataReportUserDto> Handle(GetDashBoardUserQuery request, CancellationToken cancellationToken)
        {
            var (userId, _) = _userService.GetCurrentUserAuthenticationInfo();
            if (string.IsNullOrEmpty(userId))
            {
                throw new AppException("Không tìm thấy thông tin người dùng được xác thực.", 401);
            }

            var courses = await _unitOfWork.CourseRepository.GetFlexibleAsync(
                predicate: c => c.UserCourses != null && c.UserCourses.Any(uc => uc.UserId == userId) && !c.IsDeleted,
                asNoTracking: true
            );
            var userEnrollCourseDtos = _mapper.Map<List<UserEnrollCourseDto>>(courses);

            var numberregisteredcourse = (await _unitOfWork.UserCourseRepository.GetFlexibleAsync(p => p.UserId == userId)).ToList().Count();
            var numbercompletedcourse = (await _unitOfWork.UserCourseRepository.GetFlexibleAsync(p => p.UserId == userId && p.Status == ConstantStatus.COMPLETED)).ToList().Count;

            float progressPercentage = 0;
            foreach (var courseDto in userEnrollCourseDtos)
            {
                float lessonsProgress = await GetCourseProgress(courseDto.Id, userId);
                Console.WriteLine(courseDto.Id.ToString() + ": " + lessonsProgress);
                progressPercentage += lessonsProgress;
            }

            float AverangeCompleted = userEnrollCourseDtos.Any() ? (float)Math.Round(progressPercentage / userEnrollCourseDtos.Count(), 1) : 0;

            return new DataReportUserDto
            {
                NumberRegisteredCourse = numberregisteredcourse,
                NumberCompletedCourse = numbercompletedcourse,
                AverangeCompletedPercentage = AverangeCompleted
            };
        }

        private async Task<float> CalculateLessonsProgressAsync(string courseId, string userId)
        {
            // Lấy tất cả bài học trong khóa học
            var lessons = await _unitOfWork.LessonRepository
                .GetFlexibleAsync(l => l.CourseId == courseId);

            if (lessons == null || !lessons.Any()) return 0.0f;

            int totalLessons = lessons.Count();

            // Lấy tiến độ học tập của người dùng với các bài học đó
            var lessonProgresses = await _unitOfWork.LessonProgressRepository
                .GetFlexibleAsync(lp => lp.UserId == userId && lp.Lesson.CourseId == courseId);

            float totalProgress = 0;

            foreach (var lesson in lessons)
            {
                var progress = lessonProgresses.FirstOrDefault(lp => lp.LessonId == lesson.Id);

                float lessonProgress = 0f;

                if (progress != null)
                {
                    if (progress.IsCompleted)
                    {
                        lessonProgress = 1.0f;
                    }
                    else if (lesson.TotalDurationSeconds.HasValue && progress.CurrentTimeSeconds.HasValue && lesson.TotalDurationSeconds > 0)
                    {
                        lessonProgress = (float)progress.CurrentTimeSeconds.Value / lesson.TotalDurationSeconds.Value;
                    }
                    else if (lesson.TotalPages.HasValue && progress.CurrentPage.HasValue && lesson.TotalPages > 0)
                    {
                        lessonProgress = (float)progress.CurrentPage.Value / lesson.TotalPages.Value;
                    }

                    // Đảm bảo không vượt quá 1.0f
                    lessonProgress = Math.Clamp(lessonProgress, 0f, 1f);
                }

                totalProgress += lessonProgress;
            }

            float overallProgress = totalProgress / totalLessons;
            return overallProgress;
        }
        private async Task<float> CalculateTestsProgressAsync(string courseId, string userId)
        {
            // Lấy tất cả bài kiểm tra trong khóa học
            var tests = await _unitOfWork.TestRepository
                .GetFlexibleAsync(t => t.CourseId == courseId);

            if (tests == null || !tests.Any()) return 0.0f;

            int totalTests = tests.Count();

            // Lấy kết quả bài kiểm tra của người dùng
            var testResults = await _unitOfWork.TestResultRepository.GetFlexibleAsync(
                tr => tr.UserId == userId && tr.Test != null && tr.Test.CourseId == courseId,
                orderBy: tr => tr.OrderByDescending(r => r.Score)
            );

            float totalProgress = 0;

            foreach (var test in tests)
            {
                var result = testResults.FirstOrDefault(tr => tr.TestId == test.Id);

                if (result != null)
                {
                    totalProgress += result.IsPassed ? 1.0f : 0f;
                }
            }

            float overallProgress = totalProgress / totalTests;
            return overallProgress;
        }

        public async Task<float> GetCourseProgress(string courseId, string userId)
        {
            var lessonsProgress = await CalculateLessonsProgressAsync(courseId, userId);
            // Tính toán tiến độ bài kiểm tra
            var testsProgress = await CalculateTestsProgressAsync(courseId, userId);
            var lessons = await _unitOfWork.LessonRepository
                .GetFlexibleAsync(c => c.CourseId == courseId);
            var tests = await _unitOfWork.TestRepository
                .GetFlexibleAsync(t => t.CourseId == courseId);
            float count = (float)lessons.Count() + (float)tests.Count();
            Console.WriteLine($"Lessons Progress: {lessonsProgress}, Tests Progress: {testsProgress}, Count: {count}, Lesson Count: {lessons.Count()}, Test Count: {tests.Count()}");
            // Tính toán tổng tiến độ
            float overallProgress = (lessonsProgress * (float)lessons.Count() + testsProgress * (float)tests.Count()) / count;
            if (float.IsNaN(overallProgress) || float.IsInfinity(overallProgress))
            {
                return 0.0f; // Trả về 0 nếu tiến độ không hợp lệ
            }

            return overallProgress;
        }
    }
}
