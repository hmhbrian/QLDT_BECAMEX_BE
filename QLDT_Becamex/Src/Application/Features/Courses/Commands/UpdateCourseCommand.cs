using MediatR;
using QLDT_Becamex.Src.Application.Common.Dtos;
using QLDT_Becamex.Src.Application.Features.Courses.Dtos;

namespace QLDT_Becamex.Src.Application.Features.Courses.Commands
{
    public record UpdateCourseCommand(string Id, UpdateCourseDto Request) : IRequest<string>;
}