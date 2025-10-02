using Azure.Core;
using Microsoft.EntityFrameworkCore;
using QLDT_Becamex.Src.Application.Features.Notifications.Abstractions;
using QLDT_Becamex.Src.Domain.Interfaces;
using System;

namespace QLDT_Becamex.Src.Application.Features.Notifications.Services
{
    public sealed class NotificationComposer : INotificationComposer
    {
        private readonly IUnitOfWork _unitOfWork;
        public NotificationComposer(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<(string Title, string Body, Dictionary<string, string> Data)> CourseCreated_GeneralAsync(string courseId, CancellationToken ct)
        {
            var c = await _unitOfWork.CourseRepository.GetFirstOrDefaultAsync(
                predicate: c => c.Id == courseId
            );

            var title = "Khóa học mới";
            var body = $"Có khóa học mới [{c.Code}] {c.Name}! Hãy đăng ký để có thể học.";
            var data = new Dictionary<string, string>
            {
                ["type"] = "CourseDetail",
                ["courseId"] = c.Id.ToString()
            };
            return (title, body, data);
        }

        public async Task<(string, string, Dictionary<string, string>)> CourseCreated_MandatoryAsync(string courseId, CancellationToken ct)
        {
            var c = await _unitOfWork.CourseRepository.GetFirstOrDefaultAsync(
                predicate: c => c.Id == courseId
            );

            var title = "Khóa học bắt buộc";
            var body = $"Bạn được yêu cầu tham gia: [{c.Code}] {c.Name}";
            var data = new Dictionary<string, string>
            {
                ["type"] = "CourseDetail",
                ["courseId"] = c.Id.ToString()
            };
            return (title, body, data);
        }
    }
}
