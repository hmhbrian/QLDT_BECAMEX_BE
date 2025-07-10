using MediatR;
using QLDT_Becamex.Src.Application.Features.TypeDocument.Dtos;

namespace QLDT_Becamex.Src.Application.Features.TypeDocument.Commands
{
    public record CreateTypeDocumentCommand(TypeDocumentRqDto Request) : IRequest<Unit>;
    public record UpdateTypeDocumentCommand(int id, TypeDocumentRqDto Request) : IRequest<Unit>;
    public record DeleteTypeDocumentCommand(List<int> Ids) : IRequest<Unit>;
}
