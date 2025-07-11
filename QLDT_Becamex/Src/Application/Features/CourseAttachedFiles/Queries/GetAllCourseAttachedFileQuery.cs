using MediatR;
using QLDT_Becamex.Src.Application.Features.CourseAttachedFile.Dtos;


namespace QLDT_Becamex.Src.Application.Features.CourseAttachedFiles.Queries
{

    public record GetAllCourseAttachedFileQuery(string CourseId) : IRequest<List<CourseAttachedFileDto>>;
}