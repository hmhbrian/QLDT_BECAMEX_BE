using MediatR;

namespace QLDT_Becamex.Src.Domain.Events
{
    public sealed record CourseStartingEvent
    (
        string CourseId,
        DateTime? startDate
    ) : INotification;
}