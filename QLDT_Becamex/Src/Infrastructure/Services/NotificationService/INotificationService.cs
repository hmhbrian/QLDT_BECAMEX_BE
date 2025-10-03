namespace QLDT_Becamex.Src.Infrastructure.Services.NotificationService
{
    public interface INotificationService
    {
        Task SendNotificationAsync(
            string title,
            string body,
            Dictionary<string, string> data,
            List<(int DeviceId, string Token)> tokens,
            CancellationToken ct);
    }
}
