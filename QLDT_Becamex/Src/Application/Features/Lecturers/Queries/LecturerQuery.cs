using MediatR;
using QLDT_Becamex.Src.Application.Features.Lecturer.Dtos;

namespace QLDT_Becamex.Src.Application.Features.Lecturer.Queries
{
    public record GetAlLecturerQuery : IRequest<IEnumerable<LecturerDto>>;
}
