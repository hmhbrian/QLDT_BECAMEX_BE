using MediatR;


namespace QLDT_Becamex.Src.Application.Features.CourseAttachedFiles.Commands
{

    public record DeleteCourseAttachedFileCommand(string CourseId, int CourseAttachedFileId) : IRequest<string>;
}