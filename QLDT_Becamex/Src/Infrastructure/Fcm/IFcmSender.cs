
namespace QLDT_Becamex.Src.Infrastructure.Fcm
{
    public interface IFcmSender
    {
        Task<IReadOnlyList<PerTokenResult>> SendMulticastAsync(
            string title,
            string body,
            IDictionary<string, string> data,
            IReadOnlyList<(int DeviceId, string Token)> tokens,
            CancellationToken ct);
    }

    public sealed record PerTokenResult(int DeviceId, string Token, bool Success, string? Error);
}
