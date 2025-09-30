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
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace QLDT_Becamex.Src.Application.Features.Notifications.Handlers
{
    public class GetNotificationOfUserQueryHandler : IRequestHandler<GetNotificationOfUserQuery, NotificationDto>
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
        public async Task<NotificationDto> Handle(GetNotificationOfUserQuery request, CancellationToken cancellationToken)
        {
            var (userId, role) = _userService.GetCurrentUserAuthenticationInfo();

            if (string.IsNullOrEmpty(userId))
            {
                // Sử dụng AppException của bạn với mã lỗi phù hợp
                throw new AppException("User ID not found. User must be authenticated.", 401);
            }

            var notification = await _unitOfWork.UserNotificationRepository.GetFlexibleAsync(
                predicate: l => l.UserId == userId && !l.IsHidden,
                includes: q => q.Include(l => l.Message),
                asNoTracking: true
            );

            switch (request.IsRead.Filter)
            {
                case NotificationFilter.Unread:
                    notification = notification.Where(x => !x.IsRead);
                    break;
                case NotificationFilter.Read:
                    notification = notification.Where(x => x.IsRead);
                    break;
                case NotificationFilter.All:
                default:
                    break;
            }

            var items = notification
                .OrderByDescending(x => x.SentAt)
                .ThenByDescending(x => x.Id)
                .Select(x => new NotificationItemDto
                {
                    Id = x.Id,
                    Title = x.Message!.Title,
                    Body = x.Message.Body,
                    DataJson = x.Message.Data,
                    SentAt = x.SentAt,
                    IsRead = x.IsRead,
                    ReadAt = x.ReadAt
                }).ToList();

            var unreadCount = (await _unitOfWork.UserNotificationRepository.GetFlexibleAsync(
                    predicate: x => x.UserId == userId && !x.IsHidden && !x.IsRead
                 )).ToList().Count();

            return new NotificationDto
            {
                Items = items,
                UnreadCount = unreadCount
            };
        }
    }
}
