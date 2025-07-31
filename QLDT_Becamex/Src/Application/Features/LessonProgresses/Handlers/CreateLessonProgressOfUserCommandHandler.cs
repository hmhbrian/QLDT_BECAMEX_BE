using MediatR;
using QLDT_Becamex.Src.Application.Common.Dtos;
using QLDT_Becamex.Src.Application.Features.LessonProgresses.Commands;
using QLDT_Becamex.Src.Domain.Entities;
using QLDT_Becamex.Src.Domain.Interfaces;
using QLDT_Becamex.Src.Infrastructure.Services;
using QLDT_Becamex.Src.Shared.Helpers;

namespace QLDT_Becamex.Src.Application.Features.LessonProgresses.Handlers
{
    public class CreateLessonProgressOfUserCommandHandler : IRequestHandler<CreateLessonProgressOfUserCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserService _userService;
        public CreateLessonProgressOfUserCommandHandler(IUnitOfWork unitOfWork, IUserService userService)
        {
            _unitOfWork = unitOfWork;
            _userService = userService;
        }
        public async Task Handle(CreateLessonProgressOfUserCommand request, CancellationToken cancellationToken)
        {
            // Lấy User ID từ BaseService
            var (userId, _) = _userService.GetCurrentUserAuthenticationInfo();
            if (string.IsNullOrEmpty(userId))
            {
                throw new AppException("User ID not found. User must be authenticated.", 404);
            }
            var existingLesson = await _unitOfWork.LessonRepository.GetByIdAsync(request.Request.LessonId);
            var isCompleted = false; // Mặc định là chưa hoàn thành

            if (existingLesson != null)
            {
                var currentPage = request.Request.CurrentPage > 0 ? request.Request.CurrentPage : 0; // Nếu CurrentPage không được cung cấp, mặc định là 0
                var currentTime = request.Request.CurrentTimeSecond > 0 ? request.Request.CurrentTimeSecond : 0;

                if (currentPage > 0 && currentPage == existingLesson.TotalPages)
                    isCompleted = true; // Nếu CurrentPage bằng tổng số trang thì đánh dấu là hoàn thành
                else if (currentTime > 0 && Math.Abs((existingLesson.TotalDurationSeconds ?? 0) - (currentTime ?? 0)) < 3)
                    isCompleted = true; // Nếu CurrentTimeSecond lớn hơn hoặc bằng tổng thời gian thì đánh dấu là hoàn thành
            }
            else
            {
                throw new AppException("Lesson ID not found.", 404);
            }

            var lessonprogress = await _unitOfWork.LessonProgressRepository.GetFirstOrDefaultAsync(
                predicate: c => c.UserId == userId && c.LessonId == request.Request.LessonId);

            if (lessonprogress != null)
            {
                if (lessonprogress.CurrentPage < request.Request.CurrentPage || lessonprogress.CurrentTimeSeconds < request.Request.CurrentTimeSecond)
                {
                    // Nếu đã có LessonProgress, cập nhật nó
                    var updatedLessonProgress = new LessonProgress();
                    updatedLessonProgress.UserId = userId;
                    updatedLessonProgress.Update(request.Request, isCompleted);
                    // Cập nhật vào repository
                    _unitOfWork.LessonProgressRepository.Update(lessonprogress, updatedLessonProgress);
                }
            }
            else
            {
                // Tạo mới LessonProgress
                var NewLessonProgress = new LessonProgress();

                NewLessonProgress.Create(userId, request.Request, isCompleted);
                // Thêm vào repository
                await _unitOfWork.LessonProgressRepository.AddAsync(NewLessonProgress);
            }

            // Lưu thay đổi
            await _unitOfWork.CompleteAsync();
            var lesson = await _unitOfWork.LessonRepository.GetByIdAsync(request.Request.LessonId);
            var courseId = lesson?.CourseId;
            if (courseId == null)
            {
                throw new AppException("Lesson does not belong to any course", 404);
            }
            await IsCompletedCourse(courseId, userId);
        }
        private async Task IsCompletedCourse(string courseId, string userId)
        {
            var userCourse = await _unitOfWork.UserCourseRepository.GetFirstOrDefaultAsync(uc => uc.UserId == userId && uc.CourseId == courseId);
            if (userCourse == null)
            {
                throw new AppException("Khóa học không tồn tại cho người dùng", 404);
            }
            var lessons = await _unitOfWork.LessonRepository
                .GetFlexibleAsync(c => c.CourseId == courseId);
            var lessonsProgress = await _unitOfWork.LessonProgressRepository
                .GetFlexibleAsync(lp => lp.UserId == userId && lp.Lesson.CourseId == courseId);
            if (lessonsProgress.Count() != lessons.Count())
            {
                Console.WriteLine($"Not all lessons completed for user {userId} in course {courseId}");
                userCourse.Status = "In Progress";
                await _unitOfWork.CompleteAsync();
                return;
            }
            foreach (var lesson in lessonsProgress)
            {
                if (!lesson.IsCompleted)
                {
                    Console.WriteLine($"Lesson {lesson.LessonId} is not completed for user {userId}");
                    userCourse.Status = "In Progress";
                    await _unitOfWork.CompleteAsync();
                    return;
                }
                Console.WriteLine($"Lesson {lesson.LessonId} is completed for user {userId}");
            }
            var tests = await _unitOfWork.TestRepository
                .GetFlexibleAsync(t => t.CourseId == courseId);
            var testResults = await _unitOfWork.TestResultRepository
                .GetFlexibleAsync(tr => tr.UserId == userId && tr.Test != null && tr.Test.CourseId == courseId);
            var highestScorePerTest = testResults
                .GroupBy(tr => tr.TestId)
                .Select(g => g.OrderByDescending(tr => tr.Score).First())
                .ToList();
            if (highestScorePerTest.Count() < tests.Count())
            {
                Console.WriteLine($"Not all tests completed for user {userId} in course {courseId}");
                userCourse.Status = "In Progress";
                await _unitOfWork.CompleteAsync();
                return;
            }
            foreach (var test in highestScorePerTest)
            {
                if (!test.IsPassed)
                {
                    Console.WriteLine($"Test {test.TestId} is not passed for user {userId}");
                    userCourse.Status = "In Progress";
                    await _unitOfWork.CompleteAsync();
                    return;
                }
                Console.WriteLine($"Test {test.TestId} is passed for user {userId}");
            }
            userCourse.Status = "Completed";
            Console.WriteLine($"Course {courseId} is completed for user {userId}");
            await _unitOfWork.CompleteAsync();
        }
    }
}
