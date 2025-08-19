using AutoMapper;
using CloudinaryDotNet;
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

            if(currentUserId ==  null)
                throw new AppException($"Người dùng không tồn tại", 404);
            var currentUser = await _unitOfWork.UserRepository.GetByIdAsync(currentUserId);
            
            if (role == ConstantRole.HOCVIEN)
            {
                // Default for unknown roles or no role: same as USER (or stricter if needed)
                predicate = predicate.And( c => c.IsDeleted == false && c.IsPrivate == false && c.Status!.Key > 0
                && !c.UserCourses!.Any(uc => uc.UserId == currentUserId));

                if (currentUser?.DepartmentId != null)
                {
                    predicate = predicate.And(c =>
                        c.CourseDepartments == null
                        || !c.CourseDepartments.Any() // không gán phòng ban nào
                        || c.CourseDepartments.Any(cd => cd.DepartmentId == currentUser.DepartmentId)
                    );
                }

                if (currentUser?.ELevelId != null)
                {
                    predicate = predicate.And(c =>
                        c.CourseELevels == null
                        || !c.CourseELevels.Any() // không gán ELevel nào
                        || c.CourseELevels.Any(ce => ce.ELevelId == currentUser.ELevelId)
                    );
                }
            }

          
            // Keyword
            if (!string.IsNullOrEmpty(queryParam.Keyword))
            {
                var keyword = StringHelper.RemoveDiacritics(queryParam.Keyword).ToUpperInvariant().Replace(" ", "");
                predicate = predicate.And(c => c.NormalizeCourseName.Contains(keyword) || c.Code.Contains(keyword));
            }

            int totalItems = await _unitOfWork.CourseRepository.CountAsync(predicate);


            var courses = (await _unitOfWork.CourseRepository.GetFlexibleAsync(
                predicate: predicate,
                orderBy: q => q.OrderByDescending(c => c.RegistrationClosingDate),
                page: queryParam.Page,
                pageSize: queryParam.Limit,
                asNoTracking: true,
                includes: q => q
                    .Include(c => c.Status)
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
