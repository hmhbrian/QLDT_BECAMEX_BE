using MediatR;

namespace QLDT_Becamex.Src.Domain.Events
{
    public sealed record CourseEndingEvent
    (
        string CourseId,
        DateTime? endDate
    ) : INotification;
}