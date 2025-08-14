using AutoMapper;
using MediatR;
using QLDT_Becamex.Src.Application.Common.Dtos;
using QLDT_Becamex.Src.Application.Features.Courses.Queries;
using QLDT_Becamex.Src.Domain.Interfaces;
using QLDT_Becamex.Src.Infrastructure.Services;

namespace QLDT_Becamex.Src.Application.Features.Courses.Handlers
{
    public class GetCompletedCoursesCountQueryHandler : IRequestHandler<GetCompletedCoursesCountQuery, int>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;
        public GetCompletedCoursesCountQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, IUserService userService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userService = userService;
        }

        public async Task<int> Handle(GetCompletedCoursesCountQuery request, CancellationToken cancellationToken)
        {
            // code
            var (userId, _) = _userService.GetCurrentUserAuthenticationInfo();
            if (string.IsNullOrEmpty(userId))
            {
                throw new AppException("Người dùng không hợp lệ", 400);
            }
            var courses = await _unitOfWork.CourseRepository.GetAllAsync();
            if (courses == null || !courses.Any())
            {
                return 0; // Không có khóa học nào
            }
            int count = 0;
            foreach (var course in courses)
            {
                var userCourse = await _unitOfWork.UserCourseRepository.GetFirstOrDefaultAsync(uc => uc.CourseId == course.Id && uc.UserId == userId);
                if (userCourse == null)
                {
                    continue;
                }
                if (userCourse.Status == 3)
                    count++;
            }
            return count;
        }
    }
}
