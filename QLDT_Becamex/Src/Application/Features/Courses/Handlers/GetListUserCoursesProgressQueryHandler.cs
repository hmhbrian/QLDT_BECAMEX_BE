using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using QLDT_Becamex.Src.Application.Common.Dtos;
using QLDT_Becamex.Src.Domain.Interfaces;
using QLDT_Becamex.Src.Domain.Entities;
using QLDT_Becamex.Src.Infrastructure.Services;
using QLDT_Becamex.Src.Application.Features.Courses.Queries;
using QLDT_Becamex.Src.Application.Features.Courses.Dtos;
using QLDT_Becamex.Src.Constant;

namespace QLDT_Becamex.Src.Application.Features.Courses.Handlers
{
    public class GetListUserCoursesProgressQueryHandler : IRequestHandler<GetListUserCoursesProgressQuery, PagedResult<UserCourseProgressDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserService _userService;

        public GetListUserCoursesProgressQueryHandler(
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

        //Hiển thị danh sách tiến độ của các học viên trong khóa học
        public async Task<PagedResult<UserCourseProgressDto>> Handle(GetListUserCoursesProgressQuery request, CancellationToken cancellationToken)
        {
            var queryParams = request.baseQueryParam;
            string courseId = request.courseId;
            if (string.IsNullOrEmpty(courseId))
            {
                throw new ArgumentException("Course ID cannot be null or empty.", nameof(courseId));
            }
            var userCourses = await _unitOfWork.UserCourseRepository
                .GetFlexibleAsync(uc => uc.CourseId == courseId);
            var userCoursesProgressDtos = new List<UserCourseProgressDto>();
            foreach (var userCourse in userCourses)
            {
                var user = await _userManager.FindByIdAsync(userCourse.UserId);
                if (user == null) continue;

                var progress = userCourse.PercentComplete;
                var userCourseProgressDto = new UserCourseProgressDto
                {
                    userId = user.Id,
                    userName = user.FullName != null ? user.FullName : "",
                    progressPercentage = (float)Math.Round(progress)
                };
                userCoursesProgressDtos.Add(userCourseProgressDto);
            } 
            var sortedUserCoursesProgressDtos = userCoursesProgressDtos
                .OrderByDescending(dto => dto.progressPercentage)
                .ToList();

            var pagination = new Pagination(queryParams.Page,
                queryParams.Limit,
                sortedUserCoursesProgressDtos.Count);

            // 🔽 Chỉ lấy phần dữ liệu cần hiển thị theo trang
            var pagedData = sortedUserCoursesProgressDtos.ToList();

            return new PagedResult<UserCourseProgressDto>(pagedData, pagination);
        }
    }
}
