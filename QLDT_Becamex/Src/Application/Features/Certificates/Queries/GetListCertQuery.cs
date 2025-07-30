using MediatR;
using QLDT_Becamex.Src.Application.Features.Certificates.Dtos;

namespace QLDT_Becamex.Src.Application.Features.Certificates.Queries
{
    public record GetListCertQuery() : IRequest<List<CertListDto>>;
}
