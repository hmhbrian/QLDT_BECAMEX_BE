using AutoMapper;
using CloudinaryDotNet.Actions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QLDT_Becamex.Src.Application.Common.Dtos;
using QLDT_Becamex.Src.Application.Features.Courses.Dtos;
using QLDT_Becamex.Src.Application.Features.Courses.Queries;
using QLDT_Becamex.Src.Constant;
using QLDT_Becamex.Src.Domain.Entities;
using QLDT_Becamex.Src.Domain.Interfaces;
using QLDT_Becamex.Src.Infrastructure.Services;
using QLDT_Becamex.Src.Shared.Helpers;
using System.Globalization;
using System.Linq.Expressions;

namespace QLDT_Becamex.Src.Application.Features.Courses.Handlers
{
    public class SearchPublicCourseQueryHandler : IRequestHandler<SearchPublicCourseQuery, PagedResult<CourseDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;

        public SearchPublicCourseQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, IUserService userService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userService = userService;
        }

        public async Task<PagedResult<CourseDto>> Handle(SearchPublicCourseQuery request, CancellationToken cancellationToken)
        {
            var (currentUserId, role) = _userService.GetCurrentUserAuthenticationInfo();
            var queryParam = request.QueryParam;
            Expression<Func<Course, bool>>? predicate = c => c.IsDeleted == false;

            if (role == ConstantRole.HOCVIEN)
            {
                // Default for unknown roles or no role: same as USER (or stricter if needed)
                predicate = predicate.And( c => c.IsDeleted == false && c.IsPrivate == false && c.Status!.Key == 1
                && !c.UserCourses!.Any(uc => uc.UserId == currentUserId));
            }

            // Keyword
            if (!string.IsNullOrEmpty(queryParam.Keyword))
            {
                var keyword = StringHelper.RemoveDiacritics(queryParam.Keyword).ToUpperInvariant().Replace(" ", "");
                predicate = predicate.And(c => c.NormalizeCourseName.Contains(keyword) || c.Code.Contains(keyword));
            }

            int totalItems = await _unitOfWork.CourseRepository.CountAsync(predicate);

            Func<IQueryable<Course>, IOrderedQueryable<Course>>? orderBy = q =>
            {
                bool isDesc = queryParam.SortType?.Equals("desc", StringComparison.OrdinalIgnoreCase) == true;
                return queryParam.SortField?.ToLower() switch
                {
                    "created.at" => isDesc ? q.OrderByDescending(c => c.CreatedAt) : q.OrderBy(c => c.CreatedAt),
                    _ => q.OrderBy(c => c.CreatedAt)
                };
            };
            var courses = (await _unitOfWork.CourseRepository.GetFlexibleAsync(
                predicate: predicate,
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
            )).ToList();

            var courseDtos = _mapper.Map<List<CourseDto>>(courses);

            var pagination = new Pagination(
                currentPage: queryParam.Page,
                itemsPerPage: queryParam.Limit > 0 ? queryParam.Limit : 10,
                totalItems: totalItems);

            var result = new PagedResult<CourseDto>(
                items: courseDtos,
                pagination: pagination);
            return result;
        }
    }
}
