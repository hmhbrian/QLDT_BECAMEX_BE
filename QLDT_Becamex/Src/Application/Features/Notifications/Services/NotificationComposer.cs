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
        public async Task<(string Title, string Body, Dictionary<string, string> Data)> CourseStartingSoonAsync(string courseId, CancellationToken ct)
        {
            var c = await _unitOfWork.CourseRepository.GetFirstOrDefaultAsync(
                predicate: c => c.Id == courseId
            );

            var title = "Khóa học sắp bắt đầu";
            var body = $"[{c.Code}] {c.Name} sẽ bắt đầu vào ngày {c.StartDate:dd/MM/yyyy}";
            var data = new Dictionary<string, string>
            {
                ["type"] = "CourseDetail",
                ["courseId"] = c.Id.ToString()
            };
            return (title, body, data);
        }
        public async Task<(string Title, string Body, Dictionary<string, string> Data)> CourseEndingSoonAsync(string courseId, CancellationToken ct)
        {
            var c = await _unitOfWork.CourseRepository.GetFirstOrDefaultAsync(
                predicate: c => c.Id == courseId
            );

            var title = "Khóa học sắp kết thúc";
            var body = $"[{c.Code}] {c.Name} sẽ kết thúc vào ngày {c.EndDate:dd/MM/yyyy}";
            var data = new Dictionary<string, string>
            {
                ["type"] = "CourseDetail",
                ["courseId"] = c.Id.ToString()
            };
            return (title, body, data);
        }
        public async Task<(string Title, string Body, Dictionary<string, string> Data)> CompletedCourseAsync(string courseId, CancellationToken ct)
        {
            var c = await _unitOfWork.CourseRepository.GetFirstOrDefaultAsync(
                predicate: c => c.Id == courseId
            );

            var title = "Hoàn thành khóa học";
            var body = $"[{c.Code}] {c.Name} đã hoàn thành vào ngày {c.EndDate:dd/MM/yyyy}";
            var data = new Dictionary<string, string>
            {
                ["type"] = "CourseDetail",
                ["courseId"] = c.Id.ToString()
            };
            return (title, body, data);
        }
    }
}
