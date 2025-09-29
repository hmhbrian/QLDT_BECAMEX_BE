using MediatR;
using QLDT_Becamex.Src.Application.Features.Notifications.Dtos;

namespace QLDT_Becamex.Src.Application.Features.Notifications.Queries
{
    public record GetNotificationOfUserQuery(bool? IsRead) : IRequest<NotificationDto>;
}
