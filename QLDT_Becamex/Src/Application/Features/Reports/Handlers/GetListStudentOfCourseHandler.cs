using MediatR;
using QLDT_Becamex.Src.Application.Features.Reports.Dtos;
using QLDT_Becamex.Src.Application.Features.Reports.Queries;
using QLDT_Becamex.Src.Domain.Interfaces;

namespace QLDT_Becamex.Src.Application.Features.Reports.Handlers
{
    public class GetListStudentOfCourseHandler : IRequestHandler<GetListStudentOfCourseQuery, List<StudentOfCourseDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetListStudentOfCourseHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<StudentOfCourseDto>> Handle(GetListStudentOfCourseQuery request, CancellationToken cancellationToken)
        {
            var courses = await _unitOfWork.CourseRepository.GetFlexibleAsync(asNoTracking: true);
            var userCourses = await _unitOfWork.UserCourseRepository.GetFlexibleAsync(asNoTracking: true);

            if (!courses.Any())
                return new List<StudentOfCourseDto>();

            var result = courses
                .Where(course => userCourses.Count(uc => uc.CourseId == course.Id) > 0)
                .Select(course => new StudentOfCourseDto
                {
                    CourseName = course.Name,
                    TotalStudent = userCourses.Count(uc => uc.CourseId == course.Id)
            }).ToList();
            return result;
        }
    }
}
