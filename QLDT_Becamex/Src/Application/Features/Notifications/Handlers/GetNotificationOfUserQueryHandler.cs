using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QLDT_Becamex.Src.Application.Common.Dtos;
using QLDT_Becamex.Src.Application.Features.Lessons.Dtos;
using QLDT_Becamex.Src.Application.Features.Lessons.Queries;
using QLDT_Becamex.Src.Application.Features.Notifications.Dtos;
using QLDT_Becamex.Src.Application.Features.Notifications.Queries;
using QLDT_Becamex.Src.Domain.Entities;
using QLDT_Becamex.Src.Domain.Interfaces;
using QLDT_Becamex.Src.Infrastructure.Services;

namespace QLDT_Becamex.Src.Application.Features.Notifications.Handlers
{
    public class GetNotificationOfUserQueryHandler : IRequestHandler<GetNotificationOfUserQuery, List<NotificationDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;
        public GetNotificationOfUserQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, IUserService userService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userService = userService;
        }
        public async Task<List<NotificationDto>> Handle(GetNotificationOfUserQuery request, CancellationToken cancellationToken)
        {
            var (userId, role) = _userService.GetCurrentUserAuthenticationInfo();

            if (string.IsNullOrEmpty(userId))
            {
                // Sử dụng AppException của bạn với mã lỗi phù hợp
                throw new AppException("User ID not found. User must be authenticated.", 401);
            }

            var notification = await _unitOfWork.UserNotificationRepository.GetFlexibleAsync(
                predicate: l => l.IsRead == request.IsRead,
                includes: q => q.Include(l => l.Message)
            );
            var dto = _mapper.Map<List<NotificationDto>>(notification);
            return dto;
        }
    }
}
