using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using QLDT_Becamex.Src.Application.Common.Dtos;
using QLDT_Becamex.Src.Domain.Interfaces;
using QLDT_Becamex.Src.Domain.Entities;
using QLDT_Becamex.Src.Infrastructure.Services;
using QLDT_Becamex.Src.Application.Features.Courses.Queries;
using QLDT_Becamex.Src.Application.Features.Courses.Dtos;
using System.Drawing.Printing;
using System.Data.SqlClient;

namespace QLDT_Becamex.Src.Application.Features.Courses.Handlers
{
    public class GetCompletedCoursesQueryHandler : IRequestHandler<GetCompletedCoursesQuery, PagedResult<UserEnrollCompletedCourseDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserService _userService;

        public GetCompletedCoursesQueryHandler(
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

        public async Task<PagedResult<UserEnrollCompletedCourseDto>> Handle(GetCompletedCoursesQuery request, CancellationToken cancellationToken)
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
                predicate: c => c.UserCourses != null && c.UserCourses.Any(uc => uc.UserId == userId && uc.Status == 3) && !c.IsDeleted,
                orderBy: courseOrderByFunc,
                page: queryParams.Page,
                pageSize: queryParams.Limit,
                includes: null,
                asNoTracking: true
            );
            totalItems = courses.Count();
            var pagination = new Pagination(queryParams.Page,
                queryParams.Limit,
                totalItems);
            var userEnrollCourseDtos = _mapper.Map<List<UserEnrollCompletedCourseDto>>(courses);
            var result = new PagedResult<UserEnrollCompletedCourseDto>(userEnrollCourseDtos, pagination);
            return result;
        }
    }
}
