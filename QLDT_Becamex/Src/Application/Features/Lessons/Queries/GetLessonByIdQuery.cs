using MediatR;
using static QLDT_Becamex.Src.Application.Features.Lessons.Dtos.LessonResponseDTO;

namespace QLDT_Becamex.Src.Application.Features.Lessons.Queries
{
    public record GetLessonByIdQuery(int Id) : IRequest<DetailLessonDto>;
}
