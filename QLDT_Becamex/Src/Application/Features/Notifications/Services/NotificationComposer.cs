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
        public async Task<(string Title, string Body, Dictionary<string, string> Data)> CourseCreatedAsync(string courseId, CancellationToken ct)
        {
            var c = await _unitOfWork.CourseRepository.GetFirstOrDefaultAsync(
                predicate: c => c.Id == courseId
            );

            var title = "Khóa học mới";
            var body = $"[{c.Code}] {c.Name}";
            var data = new Dictionary<string, string>
            {
                ["type"] = "CourseDetail",
                ["courseId"] = c.Id.ToString()
            };
            return (title, body, data);
        }
    }
}
