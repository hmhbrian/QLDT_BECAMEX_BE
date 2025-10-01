using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace QLDT_Becamex.Src.Application.Features.Notifications.Command
{
    public record MarkNotificationReadCommand(int NotificationId, bool IsRead) : IRequest;
}
