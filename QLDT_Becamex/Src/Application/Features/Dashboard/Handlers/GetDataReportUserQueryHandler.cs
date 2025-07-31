using MediatR;
using QLDT_Becamex.Src.Application.Common.Dtos;
using QLDT_Becamex.Src.Application.Features.Dashboard.Dtos;
using QLDT_Becamex.Src.Application.Features.Dashboard.Queries;
using QLDT_Becamex.Src.Domain.Interfaces;
using QLDT_Becamex.Src.Infrastructure.Persistence;
using QLDT_Becamex.Src.Infrastructure.Services;
using QLDT_Becamex.Src.Infrastructure.Services.UserServices;

namespace QLDT_Becamex.Src.Application.Features.Dashboard.Handlers
{
    public class GetDataReportUserQueryHandler : IRequestHandler<GetDashBoardUserQuery, DataReportUserDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ApplicationDbContext _context;
        private readonly IUserService _userService;

        public GetDataReportUserQueryHandler(
            IUnitOfWork unitOfWork,
             IUserService userService,
            ApplicationDbContext context)
        {
            _unitOfWork = unitOfWork;
            _userService = userService;
            _context = context;
        }

        public async Task<DataReportUserDto> Handle(GetDashBoardUserQuery request, CancellationToken cancellationToken)
        {
            var (userId, _) = _userService.GetCurrentUserAuthenticationInfo();
            if (string.IsNullOrEmpty(userId))
            {
                throw new AppException("Không tìm thấy thông tin người dùng được xác thực.", 401);
            }

            var numberregisteredcourse = (await _unitOfWork.UserCourseRepository.GetFlexibleAsync(p => p.UserId == userId)).ToList().Count();
            var numbercompletedcourse = (await _unitOfWork.UserCourseRepository.GetFlexibleAsync(p => p.UserId == userId && p.Status == "Completed")).ToList().Count;

            return new DataReportUserDto
            {
                NumberRegisteredCourse = numberregisteredcourse,
                NumberCompletedCourse = numbercompletedcourse,
                AverangeCompletedPercentage = numberregisteredcourse > 0 
                                ? MathF.Round((float)numbercompletedcourse / numberregisteredcourse, 1): 0f
            };
        }
    }
}
