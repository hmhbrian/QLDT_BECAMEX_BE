using FirebaseAdmin.Messaging;

namespace QLDT_Becamex.Src.Infrastructure.Fcm
{
    public sealed class FcmSender : IFcmSender
    {
        public async Task<(bool, string?)> SendByConditionAsync(
            string title, string body, IDictionary<string, string> data, string condition, CancellationToken ct)
        {
            var msg = new Message
            {
                Condition = condition,
                Notification = new Notification { Title = title, Body = body },
                Data = data?.ToDictionary(kv => kv.Key, kv => kv.Value)
            };

            var id = await FirebaseMessaging.DefaultInstance.SendAsync(msg, ct);
            return (Success: !string.IsNullOrWhiteSpace(id), Error: null);
        }
    }
}
