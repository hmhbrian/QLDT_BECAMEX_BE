using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using QLDT_Becamex.Src.Application.Common.Dtos;
using QLDT_Becamex.Src.Domain.Interfaces;
using QLDT_Becamex.Src.Domain.Entities;
using QLDT_Becamex.Src.Infrastructure.Services;
using QLDT_Becamex.Src.Application.Features.Courses.Queries;
using QLDT_Becamex.Src.Application.Features.Courses.Dtos;

namespace QLDT_Becamex.Src.Application.Features.Courses.Handlers
{
    public class GetListEnrollCourseQueryHandler : IRequestHandler<GetListEnrollCourseQuery, PagedResult<UserEnrollCourseDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserService _userService;

        public GetListEnrollCourseQueryHandler(
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

        public async Task<PagedResult<UserEnrollCourseDto>> Handle(GetListEnrollCourseQuery request, CancellationToken cancellationToken)
        {
            var queryParams = request.baseQueryParam;
            var (userId, _) = _userService.GetCurrentUserAuthenticationInfo();
            if (string.IsNullOrEmpty(userId))
            {
                throw new AppException("User not authenticated", 401);
            }
            // 1. Tổng số bản ghi
            int totalItems = await _unitOfWork.CourseRepository.CountAsync(c =>
                c.UserCourses != null && c.UserCourses.Any(uc => uc.UserId == userId) && !c.IsDeleted);

            // 2. Hàm sắp xếp cho Course
            Func<IQueryable<Course>, IOrderedQueryable<Course>> courseOrderByFunc = query =>
            {
                bool isDesc = queryParams.SortType?.Equals("desc", StringComparison.OrdinalIgnoreCase) == true;
                return queryParams.SortField?.ToLower() switch
                {
                    "name" => isDesc ? query.OrderByDescending(c => c.Name) : query.OrderBy(c => c.Name),
                    "createdat" => isDesc ? query.OrderByDescending(c => c.CreatedAt) : query.OrderBy(c => c.CreatedAt),
                    "status" => isDesc ? query.OrderByDescending(c => c.Status) : query.OrderBy(c => c.Status),
                    _ => isDesc ? query.OrderByDescending(c => c.CreatedAt) : query.OrderBy(c => c.CreatedAt)
                };
            };
            // 3. Lấy dữ liệu có phân trang
            var courses = await _unitOfWork.CourseRepository.GetFlexibleAsync(
                predicate: c => c.UserCourses != null && c.UserCourses.Any(uc => uc.UserId == userId) && !c.IsDeleted,
                orderBy: courseOrderByFunc,
                page: queryParams.Page,
                pageSize: queryParams.Limit,
                includes: null,
                asNoTracking: true
            );
            var pagination = new Pagination(queryParams.Page,
                queryParams.Limit,
                totalItems);
            var userEnrollCourseDtos = _mapper.Map<List<UserEnrollCourseDto>>(courses);
            foreach (var courseDto in userEnrollCourseDtos)
            {
                float lessonsProgress = await GetCourseProgress(courseDto.Id, userId);
                courseDto.progressPercentage = (float)Math.Round(lessonsProgress);

                var lessons = await _unitOfWork.LessonRepository
                    .GetFlexibleAsync(l => l.CourseId == courseDto.Id);
                var lessonProgresses = await _unitOfWork.LessonProgressRepository
                    .GetFlexibleAsync(lp => lp.UserId == userId && lp.Lesson.CourseId == courseDto.Id);
                courseDto.TotalLessonCount = lessons.Count();
                courseDto.LessonCompletedCount = lessonProgresses.Count(lp => lp.IsCompleted);

                var tests = await _unitOfWork.TestRepository
                    .GetFlexibleAsync(t => t.CourseId == courseDto.Id);
                var testResults = await _unitOfWork.TestResultRepository
                    .GetFlexibleAsync(tr => tr.UserId == userId && tr.Test != null && tr.Test.CourseId == courseDto.Id);
                courseDto.TotalTestCount = tests.Count();
                courseDto.TestCompletedCount = testResults.Count(tr => tr.IsPassed);

                var userCourse = await _unitOfWork.UserCourseRepository
                    .GetFirstOrDefaultAsync(uc => uc.UserId == userId && uc.CourseId == courseDto.Id);
                courseDto.Status = userCourse!.Status;
            }
            var result = new PagedResult<UserEnrollCourseDto>(userEnrollCourseDtos, pagination);
            return result;
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

            float overallProgress = totalProgress / totalLessons * 100f;
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

            float overallProgress = (totalProgress / totalTests) * 100f;
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
            Console.WriteLine($"Lessons Progress: {lessonsProgress}, Tests Progress: {testsProgress}, Count: {count}, Course Count: {lessons.Count()}, Test Count: {tests.Count()}");
            // Tính toán tổng tiến độ
            float overallProgress = (lessonsProgress * (float)lessons.Count() + testsProgress * (float)tests.Count()) / count;
            if (float.IsNaN(overallProgress) || float.IsInfinity(overallProgress))
            {
                return 0.0f; // Trả về 0 nếu tiến độ không hợp lệ
            }
            var userCourse = await _unitOfWork.UserCourseRepository
                .GetFirstOrDefaultAsync(uc => uc.UserId == userId && uc.CourseId == courseId);
            if (userCourse == null)
            {   
                throw new AppException("Khóa học không tồn tại cho người dùng", 404);
            }
            if (overallProgress == 100.0f)
            {
                // Nếu tiến độ là 100%, đánh dấu khóa học là hoàn thành
                userCourse.Status = 3;
                await _unitOfWork.CompleteAsync();
            }
            else if (overallProgress > 0.0f && overallProgress < 100.0f)
            {
                // Nếu tiến độ từ 0 đến 100, đánh dấu khóa học là đang tiến hành
                var lessonProgresses = await _unitOfWork.LessonProgressRepository
                        .GetFlexibleAsync(lp => lp.UserId == userId && lp.Lesson.CourseId == courseId);
                var testResults = await _unitOfWork.TestResultRepository
                        .GetFlexibleAsync(tr => tr.UserId == userId && tr.Test!.CourseId == courseId);
                if (lessonProgresses.Count() == lessons.Count() && testResults.Count() == tests.Count())
                {
                    userCourse.Status = 4;
                    await _unitOfWork.CompleteAsync();
                }
                else
                {
                    userCourse.Status = 2;
                    await _unitOfWork.CompleteAsync();
                }
            }
            else if (overallProgress == 0.0f)
            {
                // Nếu tiến độ là 0, đánh dấu khóa học là chưa bắt đầu
                userCourse.Status = 1;
                await _unitOfWork.CompleteAsync();
            }
            return overallProgress;
        }
    }
}
