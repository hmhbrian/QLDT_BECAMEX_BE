namespace QLDT_Becamex.Src.Application.Features.Notifications.Abstractions
{
    public interface INotificationComposer
    {
        Task<(string Title, string Body, Dictionary<string, string> Data)>
       CourseCreatedAsync(string courseId, CancellationToken ct);

        Task<(string Title, string Body, Dictionary<string, string> Data)>
       CourseStartingSoonAsync(string courseId, CancellationToken ct);

        Task<(string Title, string Body, Dictionary<string, string> Data)>
       CourseEndingSoonAsync(string courseId, CancellationToken ct);
        Task<(string Title, string Body, Dictionary<string, string> Data)>
       CompletedCourseAsync(string courseId, CancellationToken ct);
    }
}
