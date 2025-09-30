namespace QLDT_Becamex.Src.Application.Features.Notifications.Dtos
{
    public enum NotificationFilter
    {
        All = 0,
        Unread = 1,
        Read = 2
    }
    public class NotificationRequest
    {
        public NotificationFilter Filter { get; set; } = NotificationFilter.All;
    }
}
