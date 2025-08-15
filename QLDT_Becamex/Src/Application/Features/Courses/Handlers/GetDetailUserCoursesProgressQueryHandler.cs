using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using QLDT_Becamex.Src.Application.Common.Dtos;
using QLDT_Becamex.Src.Domain.Interfaces;
using QLDT_Becamex.Src.Domain.Entities;
using QLDT_Becamex.Src.Infrastructure.Services;
using QLDT_Becamex.Src.Application.Features.Courses.Queries;
using QLDT_Becamex.Src.Application.Features.Courses.Dtos;
using QLDT_Becamex.Src.Constant;
using QLDT_Becamex.Src.Infrastructure.Services.CourseServices;

namespace QLDT_Becamex.Src.Application.Features.Courses.Handlers
{
    public class GetDetailUserCoursesProgressQueryHandler : IRequestHandler<GetDetailUserCoursesProgressQuery, DetailedUserCourseProgressDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserService _userService;

        public GetDetailUserCoursesProgressQueryHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            UserManager<ApplicationUser> userManager,
            IUserService userService
            )
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userManager = userManager;
            _userService = userService;
        }

        //Hiển thị chi tiết tiến độ của 1 học viên trong khóa học
        public async Task<DetailedUserCourseProgressDto> Handle(GetDetailUserCoursesProgressQuery request, CancellationToken cancellationToken)
        {
            string userId = request.userId;
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentException("User ID cannot be null or empty.", nameof(userId));
            }
            string courseId = request.courseId;
            if (string.IsNullOrEmpty(courseId))
            {
                throw new ArgumentException("Course ID cannot be null or empty.", nameof(courseId));
            }
            var userCourse = await _unitOfWork.UserCourseRepository.GetFirstOrDefaultAsync(
                uc => uc.UserId == userId && uc.CourseId == courseId
            );
            if (userCourse == null)
            {
                throw new AppException("Khóa học không tồn tại cho người dùng", 404);
            }
            var course = await _unitOfWork.CourseRepository.GetFirstOrDefaultAsync(c => c.Id == courseId);
            if (course == null)
            {
                throw new AppException("Khóa học không tồn tại", 404);
            }
            var lessonProgresses = await _unitOfWork.LessonProgressRepository
                .GetFlexibleAsync(lp => lp.UserId == userId && lp.Lesson.CourseId == courseId);
            var lessonsProgressDtos = new List<LessonProgressDto>();
            foreach (var lessonProgress in lessonProgresses)
            {
                var lesson = await _unitOfWork.LessonRepository.GetFirstOrDefaultAsync(l => l.Id == lessonProgress.LessonId);
                if (lesson != null)
                {
                    lessonsProgressDtos.Add(new LessonProgressDto
                    {
                        LessonId = lesson.Id.ToString(),
                        LessonName = lesson.Title,
                        ProgressPercentage = lessonProgress.IsCompleted ? 100f :
                        (lesson.TotalDurationSeconds.HasValue && lessonProgress.CurrentTimeSeconds.HasValue && lesson.TotalDurationSeconds > 0 ?
                            (float)Math.Round((float)lessonProgress.CurrentTimeSeconds.Value / lesson.TotalDurationSeconds.Value * 100f, 1) :
                            (lesson.TotalPages.HasValue && lessonProgress.CurrentPage.HasValue && lesson.TotalPages > 0 ?
                                (float)Math.Round((float)lessonProgress.CurrentPage.Value / lesson.TotalPages.Value * 100f, 1) : 0f)),
                        IsCompleted = lessonProgress.IsCompleted
                    });
                }
            }
            var testResults = await _unitOfWork.TestResultRepository
                .GetFlexibleAsync(tr => tr.UserId == userId && tr.Test != null && tr.Test.CourseId == courseId);
            var tests = await _unitOfWork.TestRepository.GetFlexibleAsync(t => t.CourseId == courseId);
            var testsProgressDtos = new List<TestScoreDto>();

            var groupedTestResults = testResults
                .GroupBy(tr => tr.TestId)
                .Select(g => g.OrderByDescending(tr => tr.Score).FirstOrDefault())
                .Where(tr => tr != null);

            foreach (var testResult in groupedTestResults)
            {
                if (testResult == null) continue;
                var test = tests.FirstOrDefault(t => t.Id == testResult.TestId);
                if (test != null)
                {
                    testsProgressDtos.Add(new TestScoreDto
                    {
                        TestId = test.Id.ToString(),
                        TestName = test.Title != null ? test.Title : "Unknown Test",
                        IsPassed = testResult.IsPassed,
                        Score = testResult.Score,
                        AttemptDate = testResult.SubmittedAt
                    });
                }
            }
            var user = await _userManager.FindByIdAsync(userId);

            var detailedUserCourseProgress = new DetailedUserCourseProgressDto
            {
                userId = userId,
                userName = user?.FullName ?? "Unknown User",
                courseId = course.Id.ToString(),
                courseName = course.Name,
                progressPercentage = (float)Math.Round(userCourse.PercentComplete),
                Status = userCourse.Status != null ? userCourse.Status : 1,
                LessonProgress = lessonsProgressDtos,
                TestScore = testsProgressDtos
            };
            return detailedUserCourseProgress;
        }
    }
}
