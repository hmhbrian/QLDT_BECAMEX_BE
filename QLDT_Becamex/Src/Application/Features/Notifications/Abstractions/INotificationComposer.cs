namespace QLDT_Becamex.Src.Application.Features.Notifications.Abstractions
{
    public interface INotificationComposer
    {
        // A) Thông báo chung cho Dept/Level
        Task<(string Title, string Body, Dictionary<string, string> Data)>
            CourseCreated_GeneralAsync(string courseId, CancellationToken ct);

        // B) Thông báo cá nhân cho học viên bắt buộc
        Task<(string Title, string Body, Dictionary<string, string> Data)>
            CourseCreated_MandatoryAsync(string courseId, CancellationToken ct);
    }
}
