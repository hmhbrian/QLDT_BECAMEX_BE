using MediatR;

namespace QLDT_Becamex.Src.Domain.Events
{
    public sealed record CompletedCourseEvent
    (
        string CourseId
    ) : INotification;
}