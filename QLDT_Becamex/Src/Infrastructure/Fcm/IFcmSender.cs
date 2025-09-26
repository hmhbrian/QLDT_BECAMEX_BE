namespace QLDT_Becamex.Src.Infrastructure.Fcm
{
    public interface IFcmSender
    {
        Task<(bool Success, string? Error)> SendByConditionAsync(
            string title, string body, IDictionary<string, string> data, string condition, CancellationToken ct);
    }
}
