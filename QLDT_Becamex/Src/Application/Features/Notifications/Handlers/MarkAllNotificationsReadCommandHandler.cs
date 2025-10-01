using MediatR;
using QLDT_Becamex.Src.Application.Common.Dtos;
using QLDT_Becamex.Src.Application.Features.Notifications.Command;
using QLDT_Becamex.Src.Domain.Interfaces;
using QLDT_Becamex.Src.Infrastructure.Services;

namespace QLDT_Becamex.Src.Application.Features.Notifications.Handlers
{
    public class MarkAllNotificationsReadCommandHandler : IRequestHandler<MarkAllNotificationsReadCommand>
    {
        private readonly IUnitOfWork _uow;
        private readonly IUserService _userService;

        public MarkAllNotificationsReadCommandHandler(IUnitOfWork uow, IUserService userService)
        {
            _uow = uow;
            _userService = userService;
        }

        public async Task Handle(MarkAllNotificationsReadCommand request, CancellationToken ct)
        {
            var (userId, _) = _userService.GetCurrentUserAuthenticationInfo();
            if (string.IsNullOrEmpty(userId))
                throw new AppException("Unauthenticated", 401);

            var list = (await _uow.UserNotificationRepository
                .GetFlexibleAsync(
                    predicate: x => x.UserId == userId && 
                              !x.IsHidden && !x.IsRead
                 ))
                .ToList();

            foreach (var n in list)
            {
                n.IsRead = true;
                n.ReadAt = DateTime.UtcNow;
            }
            await _uow.CompleteAsync();
        }
    }
}
