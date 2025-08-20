using AutoMapper;
using LinqKit;
using MediatR;
using Microsoft.AspNetCore.Identity;
using QLDT_Becamex.Src.Application.Common.Dtos;
using QLDT_Becamex.Src.Application.Features.Courses.Dtos;
using QLDT_Becamex.Src.Application.Features.Courses.Queries;
using QLDT_Becamex.Src.Domain.Entities;
using QLDT_Becamex.Src.Domain.Interfaces;
using QLDT_Becamex.Src.Infrastructure.Services;
using System.Linq.Expressions;

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
            var queryParams = request.baseQueryParamMyCourse;
            var (userId, _) = _userService.GetCurrentUserAuthenticationInfo();
            if (string.IsNullOrEmpty(userId))
            {
                throw new AppException("User not authenticated", 401);
            }

            // Khởi tạo predicate cơ bản: chỉ lấy các Course chưa bị xóa (IsDeleted = false)
            Expression<Func<Course, bool>>? predicate = c => c.UserCourses != null && c.UserCourses.Any(uc => uc.UserId == userId) && !c.IsDeleted;

            // lọc theo status
            if (queryParams.status == 2)
                predicate = predicate.And(c => c.UserCourses.Any(uc => uc.UserId == userId && uc.Status <= 2));
            else if(queryParams.status > 2)
                predicate = predicate.And(c => c.UserCourses.Any(uc => uc.UserId == userId && uc.Status == queryParams.status));

            // 1. Tổng số bản ghi
            int totalItems = await _unitOfWork.CourseRepository.CountAsync(predicate);

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
                predicate: predicate,
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
                //Đếm số lượng bài học hoàn thành
                var lessons = await _unitOfWork.LessonRepository
                    .GetFlexibleAsync(l => l.CourseId == courseDto.Id);
                var lessonProgresses = await _unitOfWork.LessonProgressRepository
                    .GetFlexibleAsync(lp => lp.UserId == userId && lp.Lesson.CourseId == courseDto.Id);
                courseDto.TotalLessonCount = lessons.Count();
                courseDto.LessonCompletedCount = lessonProgresses.Count(lp => lp.IsCompleted);

                //Đếm số lượng bài kiểm tra hoàn thành
                var tests = await _unitOfWork.TestRepository
                    .GetFlexibleAsync(t => t.CourseId == courseDto.Id);
                var testResults = await _unitOfWork.TestResultRepository
                    .GetFlexibleAsync(tr => tr.UserId == userId && tr.Test != null && tr.Test.CourseId == courseDto.Id);
                courseDto.TotalTestCount = tests.Count();
                courseDto.TestCompletedCount = testResults.Count();

                //Lấy trạng thái hoàn thành khóa học của user
                var userCourse = await _unitOfWork.UserCourseRepository
                    .GetFirstOrDefaultAsync(uc => uc.UserId == userId && uc.CourseId == courseDto.Id);

                courseDto.Status = userCourse!.Status;
                courseDto.progressPercentage = (float)Math.Round(userCourse.PercentComplete);
            }
            var result = new PagedResult<UserEnrollCourseDto>(userEnrollCourseDtos, pagination);
            return result;
        }
    }
}
