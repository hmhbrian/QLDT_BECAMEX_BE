using MediatR;
using QLDT_Becamex.Src.Application.Features.TypeDocument.Dtos;

namespace QLDT_Becamex.Src.Application.Features.TypeDocument.Queries
{
    public record GetAlTypeDocumentQuery : IRequest<IEnumerable<TypeDocumentDto>>;
}
