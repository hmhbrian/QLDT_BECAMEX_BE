using MediatR;
using QLDT_Becamex.Src.Application.Features.Reports.Dtos;
using QLDT_Becamex.Src.Application.Features.Reports.Queries;
using QLDT_Becamex.Src.Domain.Entities;
using QLDT_Becamex.Src.Domain.Interfaces;

namespace QLDT_Becamex.Src.Application.Features.Reports.Handlers
{
    public class GetCourseStatusDistributionHandler : IRequestHandler<GetCourseStatusDistributionQuery, List<StatusCourseReportDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetCourseStatusDistributionHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<StatusCourseReportDto>> Handle(GetCourseStatusDistributionQuery request, CancellationToken cancellationToken)
        {
            //Lấy ds khóa học và trạng thái khóa học
            var courses = await _unitOfWork.CourseRepository.GetFlexibleAsync(asNoTracking: true) ?? new List<Course>();
            var courseStatus = await _unitOfWork.CourseStatusRepository.GetFlexibleAsync(asNoTracking: true) ?? new List<CourseStatus>();

            if(!courses.Any()) 
                return new List<StatusCourseReportDto>();

            //Tổng khóa học
            var totalCourses = courses.Count();

            var result = courseStatus
                .GroupJoin(courses,
                    cs => cs.Id,
                    c => c.StatusId,
                    (cs, courseGroup) => new
                    {
                        StatusName = cs.StatusName ?? "Unknown",
                        CourseCount = courseGroup.Count()
                    })
                .Select(g => new StatusCourseReportDto
                {
                    StatusName = g.StatusName,
                    Percent = totalCourses > 0 ? Math.Round((double)g.CourseCount / totalCourses * 100, 1) : 0// Tính phần trăm
                })
                .OrderByDescending(dto => dto.Percent) // Sắp xếp theo phần trăm giảm dần
                .ToList();
            return result;
        }
     }
}
