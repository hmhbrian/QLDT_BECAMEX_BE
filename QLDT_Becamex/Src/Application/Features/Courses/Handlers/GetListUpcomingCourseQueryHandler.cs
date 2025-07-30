using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QLDT_Becamex.Src.Application.Common.Dtos;
using QLDT_Becamex.Src.Application.Features.Courses.Dtos;
using QLDT_Becamex.Src.Application.Features.Courses.Queries;
using QLDT_Becamex.Src.Domain.Entities;
using QLDT_Becamex.Src.Domain.Interfaces;
using QLDT_Becamex.Src.Infrastructure.Services;

namespace QLDT_Becamex.Src.Application.Features.Courses.Handlers
{
    public class GetListUpcomingCourseQueryHandler : IRequestHandler<GetListUpcomingCourseQuery, List<UserUpcomingCourseDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;

        public GetListUpcomingCourseQueryHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IUserService userService
            )
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userService = userService;
        }
        public async Task<List<UserUpcomingCourseDto>> Handle(GetListUpcomingCourseQuery request, CancellationToken cancellationToken)
        {
            var (userId, _) = _userService.GetCurrentUserAuthenticationInfo();
            if (string.IsNullOrEmpty(userId))
            {
                throw new AppException("User not authenticated", 401);
            }

            var now = DateTime.UtcNow;
            var nextweek = now.AddDays(7);

            //Lấy ds khóa học sẽ bắt đầu trong khoảng 7 ngày tính từ ngày hiện tại
            var UpcomingCourse = await _unitOfWork.UserCourseRepository.GetFlexibleAsync( 
                predicate: u => u.UserId == userId &&
                                u.Course != null &&
                                u.Course.StartDate >= now &&
                                u.Course.StartDate <= nextweek,
                includes: u => u.Include( c => c.Course));

            var userUpcomingCourseDtos = UpcomingCourse
                .Select(uc => _mapper.Map<UserUpcomingCourseDto>(uc.Course))
                .ToList();

            return userUpcomingCourseDtos;
        }
    }
}
