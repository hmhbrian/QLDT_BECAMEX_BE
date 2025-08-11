using MediatR;

namespace QLDT_Becamex.Src.Application.Features.Certificates.Commands
{
    public record CreateCertCommand(string CourseId) : IRequest<string>;
}
