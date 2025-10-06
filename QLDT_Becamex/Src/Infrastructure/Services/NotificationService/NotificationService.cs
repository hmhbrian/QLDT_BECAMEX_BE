
using QLDT_Becamex.Src.Domain.Entities;
using QLDT_Becamex.Src.Domain.Interfaces;
using QLDT_Becamex.Src.Infrastructure.Fcm;
using QLDT_Becamex.Src.Shared.Helpers;
using System.Text.Json;

namespace QLDT_Becamex.Src.Infrastructure.Services.NotificationService
{
    public sealed class NotificationService : INotificationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFcmSender _fcmSender;

        public NotificationService(IUnitOfWork unitOfWork, IFcmSender fcmSender)
        {
            _unitOfWork = unitOfWork;
            _fcmSender = fcmSender;
        }
        public async Task SendNotificationAsync(string title, string body, Dictionary<string, string> data, List<(int DeviceId, string Token)> tokens, CancellationToken ct)
        {
            DateTime now = DateTimeHelper.GetVietnamTimeNow();
            var dataJson = JsonSerializer.Serialize(data);

            //1)Tạo message
            var msg = new Messages
            {
                Title = title,
                Body = body,
                Data = dataJson,
                SendType = "Token",
                SentBy = "System",
                CreatedAt = now
            };
            await _unitOfWork.MessagesRepository.AddAsync(msg);
            await _unitOfWork.CompleteAsync();

            // 2) Tạo UserNotifications (fan-out theo user)
            var deviceIds = tokens.Select(t => t.DeviceId).Distinct().ToList();

            //Lấy devices theo Ids
            var devices = await _unitOfWork.DevicesRepository.GetFlexibleAsync(
                    predicate: d => deviceIds.Contains(d.Id)
                );
            var userIds = devices.Select(d => d.UserId).Distinct().ToList();

            // chunk để AddRange theo số lượng lớn
            const int chunkSize = 1000;
            foreach (var chunk in userIds.Chunk(chunkSize))
            {
                var rows = chunk.Select(uid => new UserNotification
                {
                    UserId = uid,
                    MessageId = msg.Id,
                    SentAt = now,
                    IsRead = false,
                    ReadAt = null,
                    IsHidden = false
                });

                await _unitOfWork.UserNotificationRepository.AddRangeAsync(rows);
                await _unitOfWork.CompleteAsync();
            }

            // 3) Gửi theo batch (IFcmSender đã tự chunk 500 token/lần)
            var perTokenResults = await _fcmSender.SendMulticastAsync(title, body, data, tokens, ct);

            // 4) Ghi log từng device
            foreach (var r in perTokenResults)
            {
                await _unitOfWork.MessageLogsRepository.AddAsync(new MessageLogs
                {
                    MessageId = msg.Id,
                    DeviceId = r.DeviceId,
                    TopicId = null,
                    Status = r.Success ? "Sent" : "Failed",
                    ErrorMessage = r.Success ? null : r.Error,
                    SentAt = DateTime.UtcNow
                });

                // token invalid -> có thể đánh Inactive
                if (!r.Success && r.Error is string err && err.Contains("registration-token-not-registered", StringComparison.OrdinalIgnoreCase))
                {
                    var device = await _unitOfWork.DevicesRepository.GetByIdAsync(r.DeviceId);
                    if (device != null)
                        Console.WriteLine(r.DeviceId + "không hoạt động");
                    _unitOfWork.DevicesRepository.Remove(device);
                }
            }
            await _unitOfWork.CompleteAsync();
        }
    }
}
