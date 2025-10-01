using MediatR;

namespace QLDT_Becamex.Src.Application.Features.Notifications.Command
{
    // Đánh dấu tất cả thông báo của user là đã đọc
    public record MarkAllNotificationsReadCommand : IRequest;
}
