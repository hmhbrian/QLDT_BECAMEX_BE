namespace QLDT_Becamex.Src.Application.Features.Notifications.Abstractions
{
    public interface IReviewReminderComposer
    {
        (string Title, string Body, Dictionary<string, string> Data)
        Build(string fullName, string courseTitle, string courseId);
    }
}
