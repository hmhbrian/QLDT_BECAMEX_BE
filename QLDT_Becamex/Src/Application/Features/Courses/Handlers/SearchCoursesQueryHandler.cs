using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QLDT_Becamex.Src.Application.Common.Dtos;
using QLDT_Becamex.Src.Application.Features.Courses.Dtos;
using QLDT_Becamex.Src.Application.Features.Courses.Queries;
using QLDT_Becamex.Src.Domain.Entities;
using QLDT_Becamex.Src.Domain.Interfaces;
using QLDT_Becamex.Src.Shared.Helpers;
using System.Globalization;
using System.Linq.Expressions;

namespace QLDT_Becamex.Src.Application.Features.Courses.Handlers
{
    public class SearchCoursesQueryHandler : IRequestHandler<SearchCoursesQuery, PagedResult<CourseDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public SearchCoursesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PagedResult<CourseDto>> Handle(SearchCoursesQuery request, CancellationToken cancellationToken)
        {
            var queryParam = request.QueryParam;
            var predicate = BuildPredicate(queryParam);

            // Keyword
            if (!string.IsNullOrEmpty(queryParam.Keyword))
            {
                var keyword = StringHelper.RemoveDiacritics(queryParam.Keyword).ToUpperInvariant().Replace(" ", "");
                predicate = predicate.And(c => c.NormalizeCourseName.Contains(keyword) || c.Code.Contains(keyword));
                Console.WriteLine("KEYWORD" + keyword);
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
                predicate: predicate, // Use the adjusted predicate here
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

        private Expression<Func<Course, bool>> BuildPredicate(BaseQueryParamFilter queryParam)
        {
            // Khởi tạo predicate cơ bản: chỉ lấy các Course chưa bị xóa (IsDeleted = false)
            Expression<Func<Course, bool>>? predicate = c => c.IsDeleted == false;

            // StatusIds
            var statusIds = !string.IsNullOrEmpty(queryParam.StatusIds)
                ? new HashSet<int>(queryParam.StatusIds.Split(',').Select(s => int.TryParse(s.Trim(), out var id) ? id : -1).Where(id => id != -1))
                : null;
            if (statusIds?.Count > 0)
                predicate = predicate.And(c => statusIds.Contains(c.Status!.Id));

            // DepartmentIds
            var deptIds = !string.IsNullOrEmpty(queryParam.DepartmentIds)
                ? new HashSet<int>(queryParam.DepartmentIds.Split(',').Select(s => int.TryParse(s.Trim(), out var id) ? id : -1).Where(id => id != -1))
                : null;
            if (deptIds?.Count > 0)
                predicate = predicate.And(c => c.CourseDepartments != null && c.CourseDepartments.Any(cd => deptIds.Contains(cd.DepartmentId)));

            // ELevelIds
            var eLevelIds = !string.IsNullOrEmpty(queryParam.ELevelIds)
                ? new HashSet<int>(queryParam.ELevelIds.Split(',').Select(s => int.TryParse(s.Trim(), out var id) ? id : -1).Where(id => id != -1))
                : null;
            if (eLevelIds?.Count > 0)
                predicate = predicate.And(c => c.CourseELevels != null && c.CourseELevels.Any(cp => eLevelIds.Contains(cp.ELevelId)));

            // CategoryIds
            var categoryIds = !string.IsNullOrEmpty(queryParam.CategoryIds)
                ? new HashSet<int>(queryParam.CategoryIds.Split(',').Select(s => int.TryParse(s.Trim(), out var id) ? id : -1).Where(id => id != -1))
                : null;
            if (categoryIds?.Count > 0)
                predicate = predicate.And(c => categoryIds.Contains(c.Category!.Id));


            // Filter by CreatedAt
            if (!string.IsNullOrEmpty(queryParam.FromDate) || !string.IsNullOrEmpty(queryParam.ToDate))
            {
                DateTime.TryParseExact(queryParam.FromDate, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var fromDate);
                DateTime.TryParseExact(queryParam.ToDate, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var toDate);
                toDate = toDate == default ? DateTime.MaxValue : toDate.AddDays(1).AddTicks(-1);
                fromDate = fromDate == default ? DateTime.MinValue : fromDate;

                Expression<Func<Course, bool>> datePredicate = c => c.CreatedAt >= fromDate && c.CreatedAt <= toDate;
                predicate = predicate == null ? datePredicate : predicate.And(datePredicate);
            }

            return predicate;
        }
    }
}
