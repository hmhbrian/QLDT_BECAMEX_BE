namespace QLDT_Becamex.Src.Application.Features.Notifications.Abstractions
{
    public interface INotificationComposer
    {
        Task<(string Title, string Body, Dictionary<string, string> Data)>
       CourseCreatedAsync(string courseId, CancellationToken ct);
    }
}
