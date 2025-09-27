using FirebaseAdmin.Messaging;

namespace QLDT_Becamex.Src.Infrastructure.Fcm
{
    public sealed class FcmSender : IFcmSender
    {
        public async Task<IReadOnlyList<PerTokenResult>> SendMulticastAsync(
        string title,
        string body,
        IDictionary<string, string> data,
        IReadOnlyList<(int DeviceId, string Token)> tokens,
        CancellationToken ct)
        {
            var results = new List<PerTokenResult>(tokens.Count);

            // loại bỏ token trùng
            var distinct = tokens
                .GroupBy(t => t.Token)
                .Select(g => g.First())
                .ToList();

            foreach (var t in distinct)
            {
                try
                {
                    var msg = new Message
                    {
                        Token = t.Token,
                        Notification = new Notification { Title = title, Body = body },
                        Data = data?.ToDictionary(kv => kv.Key, kv => kv.Value) ?? new Dictionary<string, string>()
                    };

                    var messageId = await FirebaseMessaging.DefaultInstance.SendAsync(msg, ct);

                    results.Add(new PerTokenResult(
                        t.DeviceId,
                        t.Token,
                        Success: true,
                        Error: null
                    ));
                }
                catch (FirebaseMessagingException ex)
                {
                    // Phân loại nhanh 1 số lỗi hay gặp
                    var code = ex.ErrorCode.ToString(); // Unregistered, SenderIdMismatch, Unavailable, Internal, InvalidArgument, QuotaExceeded...
                    results.Add(new PerTokenResult(
                        t.DeviceId,
                        t.Token,
                        Success: false,
                        Error: $"{code}: {ex.Message}"
                    ));
                }
                catch (Exception ex)
                {
                    results.Add(new PerTokenResult(
                        t.DeviceId,
                        t.Token,
                        Success: false,
                        Error: ex.Message
                    ));
                }
            }

            return results;
        }
    }
}
