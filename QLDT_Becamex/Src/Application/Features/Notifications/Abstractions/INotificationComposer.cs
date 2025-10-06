namespace QLDT_Becamex.Src.Application.Features.Notifications.Abstractions
{
    public interface INotificationComposer
    {
        Task<(string Title, string Body, Dictionary<string, string> Data)>
       CourseStartingSoonAsync(string courseId, CancellationToken ct);

        Task<(string Title, string Body, Dictionary<string, string> Data)>
       CourseEndingSoonAsync(string courseId, CancellationToken ct);
        Task<(string Title, string Body, Dictionary<string, string> Data)>
       CompletedCourseAsync(string courseId, CancellationToken ct);
        // A) Thông báo chung cho các học viên trong khoa/khối
        Task<(string Title, string Body, Dictionary<string, string> Data)>
        CourseCreated_GeneralAsync(string courseId, CancellationToken ct);

        // B) Thông báo cá nhân cho học viên bắt buộc
        Task<(string Title, string Body, Dictionary<string, string> Data)>
        CourseCreated_MandatoryAsync(string courseId, CancellationToken ct);
    }
}
