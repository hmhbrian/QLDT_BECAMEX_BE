// Handlers/GetCoursesQueryHandler.cs
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QLDT_Becamex.Src.Application.Common.Dtos;
using QLDT_Becamex.Src.Application.Features.Courses.Dtos;
using QLDT_Becamex.Src.Application.Features.Courses.Queries;
using QLDT_Becamex.Src.Domain.Entities;
using QLDT_Becamex.Src.Domain.Interfaces;

namespace QLDT_Becamex.Src.Application.Features.Courses.Handlers
{
    public class GetCoursesQueryHandler : IRequestHandler<GetListCourseQuery, PagedResult<CourseDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetCoursesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PagedResult<CourseDto>> Handle(GetListCourseQuery request, CancellationToken cancellationToken)
        {
            var queryParam = request.QueryParam;

            int totalItems = await _unitOfWork.CourseRepository.CountAsync(c => c.IsDeleted == false);

            Func<IQueryable<Course>, IOrderedQueryable<Course>>? orderBy = query =>
            {
                bool isDesc = queryParam.SortType?.Equals("desc", StringComparison.OrdinalIgnoreCase) == true;

                return queryParam.SortField?.ToLower() switch
                {
                    "name" => isDesc ? query.OrderByDescending(c => c.Name) : query.OrderBy(c => c.Name),
                    "created.at" => isDesc ? query.OrderByDescending(c => c.CreatedAt) : query.OrderBy(c => c.CreatedAt),
                    _ => query.OrderBy(c => c.Name)
                };
            };

            var courseEntities = await _unitOfWork.CourseRepository.GetFlexibleAsync(
                predicate: c => c.IsDeleted == false,
                orderBy: orderBy,
                page: queryParam.Page,
                pageSize: queryParam.Limit,
                asNoTracking: true,
                includes: q => q
                    .Include(c => c.CourseDepartments)!
                        .ThenInclude(cd => cd.Department)
                    .Include(c => c.CoursePositions)!
                        .ThenInclude(cp => cp.Position)
                    .Include(c => c.Status)
                    .Include(c => c.Category)
                    .Include(c => c.Lecturer)
            );

            var pagination = new Pagination
            {
                TotalItems = totalItems,
                ItemsPerPage = queryParam.Limit,
                CurrentPage = queryParam.Page,
                TotalPages = (int)Math.Ceiling((double)totalItems / queryParam.Limit)
            };

            var courseDtos = _mapper.Map<List<CourseDto>>(courseEntities);

            var pagedResult = new PagedResult<CourseDto>
            {
                Items = courseDtos,
                Pagination = pagination
            };

            return pagedResult;
        }
    }
}
