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
            Expression<Func<Course, bool>>? predicate = c => c.IsDeleted == false;

            // Filter by StatusIds
            if (!string.IsNullOrEmpty(queryParam.StatusIds))
            {
                var statusIds = queryParam.StatusIds.Split(',')
                    .Select(s => int.TryParse(s.Trim(), out var id) ? id : -1)
                    .Where(id => id != -1).ToList();
                if (statusIds.Any())
                {
                    Expression<Func<Course, bool>> statusPredicate = c => statusIds.Contains(c.Status!.Id);
                    predicate = predicate == null ? statusPredicate : predicate.And(statusPredicate);
                }
            }

            // Filter by DepartmentIds
            if (!string.IsNullOrEmpty(queryParam.DepartmentIds))
            {
                var deptIds = queryParam.DepartmentIds.Split(',')
                    .Select(s => int.TryParse(s.Trim(), out var id) ? id : -1)
                    .Where(id => id != -1).ToList();
                if (deptIds.Any())
                {
                    Expression<Func<Course, bool>> deptPredicate = c => c.CourseDepartments != null && c.CourseDepartments.Any(cd => deptIds.Contains(cd.DepartmentId));
                    predicate = predicate == null ? deptPredicate : predicate.And(deptPredicate);
                }
            }

            // Filter by PositionIds
            if (!string.IsNullOrEmpty(queryParam.ELevelIds))
            {
                var ELevelIds = queryParam.ELevelIds.Split(',')
                    .Select(s => int.TryParse(s.Trim(), out var id) ? id : -1)
                    .Where(id => id != -1).ToList();
                if (ELevelIds.Any())
                {
                    Expression<Func<Course, bool>> posPredicate = c => c.CourseELevels != null && c.CourseELevels.Any(cp => ELevelIds.Contains(cp.ELevelId));
                    predicate = predicate == null ? posPredicate : predicate.And(posPredicate);
                }
            }

            // Filter by CategoryId
            if (!string.IsNullOrEmpty(queryParam.CategoryIds))
            {
                var CategoryId = queryParam.CategoryIds.Split(',')
                    .Select(s => int.TryParse(s.Trim(), out var id) ? id : -1)
                    .Where(id => id != -1).ToList();
                if (CategoryId.Any())
                {
                    Expression<Func<Course, bool>> CategoryPredicate = c => CategoryId.Contains(c.Category!.Id);
                    predicate = predicate == null ? CategoryPredicate : predicate.And(CategoryPredicate);
                }
            }

            // Filter by LecturerId
            if (!string.IsNullOrEmpty(queryParam.LecturerIds))
            {
                var LecturerId = queryParam.LecturerIds.Split(',')
                    .Select(s => int.TryParse(s.Trim(), out var id) ? id : -1)
                    .Where(id => id != -1).ToList();
                if (LecturerId.Any())
                {
                    Expression<Func<Course, bool>> LecturerPredicate = c => LecturerId.Contains(c.Lecturer!.Id);
                    predicate = predicate == null ? LecturerPredicate : predicate.And(LecturerPredicate);
                }
            }

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

            Func<IQueryable<Course>, IOrderedQueryable<Course>>? orderBy = q =>
            {
                bool isDesc = queryParam.SortType?.Equals("desc", StringComparison.OrdinalIgnoreCase) == true;
                return queryParam.SortField?.ToLower() switch
                {
                    "name" => isDesc ? q.OrderByDescending(c => c.Name) : q.OrderBy(c => c.Name),
                    "createdat" => isDesc ? q.OrderByDescending(c => c.CreatedAt) : q.OrderBy(c => c.CreatedAt),
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
                    .Include(c => c.Lecturer)
                    .Include(c => c.CreateBy)
                    .Include(c => c.UpdateBy)
            )).ToList();

            if (!string.IsNullOrEmpty(queryParam.Keyword))
            {
                var keyword = StringHelper.RemoveDiacritics(queryParam.Keyword).ToLowerInvariant().Trim();
                courses = courses.Where(c =>
                    StringHelper.RemoveDiacritics(c.Name ?? "").ToLowerInvariant().Contains(keyword) ||
                    StringHelper.RemoveDiacritics(c.Code ?? "").ToLowerInvariant().Contains(keyword)
                ).ToList();
            }

            var totalItems = courses.Count;
            int page = queryParam.Page;
            int limit = queryParam.Limit > 0 ? queryParam.Limit : 10;
            var paged = courses.Skip((page - 1) * limit).Take(limit).ToList();

            var items = _mapper.Map<List<CourseDto>>(courses);
            var pagination = new Pagination(page, limit, totalItems);
            var result = new PagedResult<CourseDto>(items, pagination);
            return result;

        }
    }
}
