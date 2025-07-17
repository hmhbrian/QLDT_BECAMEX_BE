using AutoMapper;
using MediatR;
using QLDT_Becamex.Src.Application.Common.Dtos;
using QLDT_Becamex.Src.Application.Features.Courses.Queries;
using QLDT_Becamex.Src.Domain.Interfaces;
using QLDT_Becamex.Src.Infrastructure.Services;

namespace QLDT_Becamex.Src.Application.Features.Courses.Handlers
{
    public class GetCourseProgressQueryHandler : IRequestHandler<GetCourseProgressQuery, float>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;
        public GetCourseProgressQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, IUserService userService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userService = userService;
        }

        public async Task<float> Handle(GetCourseProgressQuery request, CancellationToken cancellationToken)
        {
            // code
            var (userId, _) = _userService.GetCurrentUserAuthenticationInfo();
            var courseExists = await _unitOfWork.CourseRepository.AnyAsync(c => c.Id == request.Id);
            if (!courseExists)
            {
                throw new AppException("Khóa học không tồn tại", 404);
            }
            if (string.IsNullOrEmpty(userId))
            {
                throw new AppException("Người dùng không hợp lệ", 400);
            }
            // Tính toán tiến độ bài học
            return await GetCourseProgress(request, userId);
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

            float overallProgress = (totalProgress / totalLessons) * 100f;
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
            var testResults = await _unitOfWork.TestResultRepository
                .GetFlexibleAsync(tr => tr.UserId == userId && tr.Test != null && tr.Test.CourseId == courseId);

            float totalProgress = 0;

            foreach (var test in tests)
            {
                var result = testResults.FirstOrDefault(tr => tr.TestId == test.Id);

                if (result != null)
                {
                    totalProgress += result.IsPassed ? 1.0f : 0f;
                }
            }

            float overallProgress = (totalProgress / totalTests) * 100f;
            return overallProgress;
        }
        public async Task<float> GetCourseProgress(GetCourseProgressQuery request, string userId)
        {
            var lessonsProgress = await CalculateLessonsProgressAsync(request.Id, userId);
            // Tính toán tiến độ bài kiểm tra
            var testsProgress = await CalculateTestsProgressAsync(request.Id, userId);
            // Tính toán tổng tiến độ
            float overallProgress = 0.0f;
            if (lessonsProgress == 0.0f) 
                overallProgress = testsProgress;
            else if (testsProgress == 0.0f)
                overallProgress = lessonsProgress;
            else
                overallProgress = (lessonsProgress + testsProgress) / 2.0f;
            if (float.IsNaN(overallProgress) || float.IsInfinity(overallProgress))
            {
                return 0.0f; // Trả về 0 nếu tiến độ không hợp lệ
            }
            var userCourse = await _unitOfWork.UserCourseRepository
                .GetFirstOrDefaultAsync(uc => uc.UserId == userId && uc.CourseId == request.Id);
            if (userCourse == null)
            {   
                throw new AppException("Khóa học không tồn tại cho người dùng", 404);
            }
            if (overallProgress == 100.0f)
            {
                // Nếu tiến độ là 100%, đánh dấu khóa học là hoàn thành
                userCourse.Status = "Completed";
                await _unitOfWork.CompleteAsync();
            }
            else
            {
                userCourse.Status = "In Progress";
                await _unitOfWork.CompleteAsync();
            }
            return overallProgress;
        }
    }
}
