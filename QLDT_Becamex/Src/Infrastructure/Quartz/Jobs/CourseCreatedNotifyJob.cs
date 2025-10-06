using Newtonsoft.Json.Linq;
using QLDT_Becamex.Src.Application.Features.Notifications.Abstractions;
using QLDT_Becamex.Src.Domain.Entities;
using QLDT_Becamex.Src.Domain.Interfaces;
using QLDT_Becamex.Src.Infrastructure.Fcm;
using QLDT_Becamex.Src.Infrastructure.Services.NotificationService;
using QLDT_Becamex.Src.Shared.Helpers;
using Quartz;
using System.Text.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace QLDT_Becamex.Src.Infrastructure.Quartz.Jobs
{
    [DisallowConcurrentExecution]
    public sealed class CourseCreatedNotifyJob : IJob
    {
        private readonly INotificationService _notificationService;
        private readonly INotificationComposer _notificationComposer;
        private readonly IRecipientResolver _resolver;
        private readonly IConfiguration _cfg;

        public CourseCreatedNotifyJob(
            INotificationService notificationService, INotificationComposer notificationComposer, IRecipientResolver resolver,
              IConfiguration cfg)
        {
            _notificationService = notificationService;
            _notificationComposer = notificationComposer;
            _resolver = resolver;
            _cfg = cfg;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var map = context.MergedJobDataMap;

            var courseId = map.GetString("CourseId")!;
            var deptIds = (map.GetString("DepartmentIds") ?? "")
                            .Split(',', StringSplitOptions.RemoveEmptyEntries)
                            .ToList();
            var levels = (map.GetString("Levels") ?? "")
                            .Split(',', StringSplitOptions.RemoveEmptyEntries)
                            .ToList();
            var mandatoryUserIds = (map.GetString("MandatoryUserIds") ?? "")
                        .Split(',', StringSplitOptions.RemoveEmptyEntries)
                        .ToList();
            // -------- 1) Resolve recipients
            var mandatoryTokens = await _resolver.ResolveByUserIdsAsync(mandatoryUserIds, context.CancellationToken);
            var generalTokens = await _resolver.ResolveByDeptLevelAsync(deptIds, levels, context.CancellationToken);

            // Loại token trùng: ưu tiên nội dung Mandatory
            var mandatorySet = new HashSet<string>(mandatoryTokens.Select(t => t.Token));
            generalTokens = generalTokens.Where(t => !mandatorySet.Contains(t.Token)).ToList();

            // Nếu tất cả đều trống thì kết thúc
            if (mandatoryTokens.Count == 0 && generalTokens.Count == 0) return;

            //---Gửi Mandatory (nếu có)
            if (mandatoryTokens.Count > 0)
            {
                //Soạn payload
                var (title, body, data) = await _notificationComposer.CourseCreated_MandatoryAsync(courseId, context.CancellationToken);

                await _notificationService.SendNotificationAsync(title, body, data, mandatoryTokens, context.CancellationToken);
            }

            //---Gửi General
            if (generalTokens.Count > 0)
            {
                //Soạn payload
                var (title, body, data) = await _notificationComposer.CourseCreated_GeneralAsync(courseId, context.CancellationToken);

                await _notificationService.SendNotificationAsync(title, body, data, generalTokens, context.CancellationToken);
            }
        }
    }
}