using MediatR;
using QLDT_Becamex.Src.Application.Features.CourseAttachedFile.Dtos;

namespace QLDT_Becamex.Src.Application.Features.CourseAttachedFiles.Commands
{
 
    public record CreateCourseAttachedFileCommand(string CourseId, List<CreateCourseAttachedFileDto> Request) : IRequest<List<CourseAttachedFileDto>>; 
}