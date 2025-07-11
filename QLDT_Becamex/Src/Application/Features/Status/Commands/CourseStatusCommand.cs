using MediatR;
using QLDT_Becamex.Src.Application.Features.Status.Dtos;

namespace QLDT_Becamex.Src.Application.Features.Status.Commands
{
    public record CreateCourseStatusCommand(CreateStatusDto Request) : IRequest<Unit>;
    public record DeleteCourseStatusesCommand(List<int> Ids) : IRequest<Unit>;
    public record UpdateCourseStatusCommand(int Id, CreateStatusDto Request) : IRequest<Unit>;
}
