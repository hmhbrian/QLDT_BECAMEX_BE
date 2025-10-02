using MediatR;

namespace QLDT_Becamex.Src.Domain.Events
{
    public sealed record CourseCreatedEvent
    (
        string CourseId,
        IReadOnlyCollection<string> DepartmentIds,// có thể rỗng
        IReadOnlyCollection<string> Levels,// có thể rỗng
        string CreatedBy
    ) : INotification;
}