using MediatR;
using QLDT_Becamex.Src.Application.Common.Dtos;
using QLDT_Becamex.Src.Application.Features.Notifications.Command;
using QLDT_Becamex.Src.Domain.Interfaces;
using QLDT_Becamex.Src.Infrastructure.Services;

namespace QLDT_Becamex.Src.Application.Features.Notifications.Handlers
{
    public class MarkNotificationReadCommandHandler : IRequestHandler<MarkNotificationReadCommand>
    {
        private readonly IUnitOfWork _uow;
        private readonly IUserService _userService;

        public MarkNotificationReadCommandHandler(IUnitOfWork uow, IUserService userService)
        {
            _uow = uow;
            _userService = userService;
        }

        public async Task Handle(MarkNotificationReadCommand request, CancellationToken ct)
        {
            var (userId, _) = _userService.GetCurrentUserAuthenticationInfo();
            if (string.IsNullOrEmpty(userId))
                throw new AppException("Unauthenticated", 401);

            var item = await _uow.UserNotificationRepository.GetByIdAsync(request.NotificationId);
            if (item is null || item.UserId != userId)
                throw new AppException("Notification not found", 404);

            if (request.IsRead)
            {
                if (!item.IsRead)
                {
                    item.IsRead = true;
                    item.ReadAt = DateTime.UtcNow;
                }
            }
            else
            {
                // đánh dấu CHƯA đọc
                if (item.IsRead)
                {
                    item.IsRead = false;
                    item.ReadAt = null;
                }
            }

            await _uow.CompleteAsync();
        }
    }
}
