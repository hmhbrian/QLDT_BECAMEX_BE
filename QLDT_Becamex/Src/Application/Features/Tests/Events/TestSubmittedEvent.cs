using MediatR;

namespace QLDT_Becamex.Src.Application.Features.Tests.Events
{
    public record TestSubmittedEvent(string UserId, string CourseId) : INotification;

}
