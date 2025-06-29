using MediatR;
using QLDT_Becamex.Src.Application.Features.Lecturer.Dtos;

namespace QLDT_Becamex.Src.Application.Features.Lecturer.Commands
{
    public record CreateLecturerCommand(LecturerDtoRq Request) : IRequest<Unit>;
    public record UpdateLecturerCommand(int id, LecturerDtoRq Request) : IRequest<Unit>;
    public record DeleteLecturerCommand(List<int> Ids) : IRequest<Unit>;
}
