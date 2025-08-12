// Handlers/GetCoursesQueryHandler.cs
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QLDT_Becamex.Src.Application.Common.Dtos;
using QLDT_Becamex.Src.Application.Features.Courses.Dtos;
using QLDT_Becamex.Src.Application.Features.Courses.Queries;
using QLDT_Becamex.Src.Constant;
using QLDT_Becamex.Src.Domain.Entities;
using QLDT_Becamex.Src.Domain.Interfaces;
using QLDT_Becamex.Src.Infrastructure.Services;
using System.Linq.Expressions;

namespace QLDT_Becamex.Src.Application.Features.Courses.Handlers
{
    public class GetListCourseQueryHandler : IRequestHandler<GetListCourseQuery, PagedResult<CourseDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;
        public GetListCourseQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, IUserService userService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userService = userService;
        }

        public async Task<PagedResult<CourseDto>> Handle(GetListCourseQuery request, CancellationToken cancellationToken)
        {
            var queryParam = request.QueryParam;
            var (currentUserId, role) = _userService.GetCurrentUserAuthenticationInfo();

            // Define the base predicate for all roles
            Expression<Func<Course, bool>> basePredicate = c => true;

            // Adjust the predicate based on the user's role
            if (role == ConstantRole.ADMIN || role == ConstantRole.MANAGER)
            {
                basePredicate = c => c.IsDeleted == false;
            }
            else
            {
                // Default for unknown roles or no role: same as USER (or stricter if needed)
                basePredicate = c => c.IsDeleted == false && c.IsPrivate == false && c.Status.Key > 0
                && !c.UserCourses.Any(uc => uc.UserId == currentUserId);
            }

            int totalItems = await _unitOfWork.CourseRepository.CountAsync(basePredicate);

            Func<IQueryable<Course>, IOrderedQueryable<Course>>? orderBy = query =>
            {
                bool isDesc = queryParam.SortType?.Equals("desc", StringComparison.OrdinalIgnoreCase) == true;

                return queryParam.SortField?.ToLower() switch
                {
                    "name" => isDesc ? query.OrderByDescending(c => c.Name) : query.OrderBy(c => c.Name),
                    "created.at" => isDesc ? query.OrderByDescending(c => c.CreatedAt) : query.OrderBy(c => c.CreatedAt),
                    _ => query.OrderBy(c => c.CreatedAt)
                };
            };

            var courseEntities = await _unitOfWork.CourseRepository.GetFlexibleAsync(
                predicate: basePredicate, // Use the adjusted predicate here
                orderBy: orderBy,
                page: queryParam.Page,
                pageSize: queryParam.Limit,
                asNoTracking: true,
                includes: q => q
                    .Include(c => c.CourseDepartments)!
                        .ThenInclude(cd => cd.Department)
                    .Include(c => c.CourseELevels)!
                        .ThenInclude(cp => cp.ELevel)
                    .Include(c => c.Status)
                    .Include(c => c.Category)
                    .Include(c => c.CreateBy)
                    .Include(c => c.UpdateBy)
            );

            // 1. Map dữ liệu
            var courseDtos = _mapper.Map<List<CourseDto>>(courseEntities);

            // 2. Tạo đối tượng phân trang
            var pagination = new Pagination(
                currentPage: queryParam.Page,
                itemsPerPage: queryParam.Limit,
                totalItems: totalItems
            );

            // 3. Tạo kết quả phân trang
            var pagedResult = new PagedResult<CourseDto>(
                items: courseDtos,
                pagination: pagination
            );

            // 4. Trả về
            return pagedResult;
        }
    }
}
