using MediatR;
using QLDT_Becamex.Src.Application.Features.Reports.Dtos;
using QLDT_Becamex.Src.Application.Features.Reports.Queries;
using QLDT_Becamex.Src.Domain.Interfaces;

namespace QLDT_Becamex.Src.Application.Features.Reports.Handlers
{
    public class GetDepartmentCourseReportHandler : IRequestHandler<GetDepartmentCourseReportQuery, List<DepartmentCourseReportDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetDepartmentCourseReportHandler(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }

        public async Task<List<DepartmentCourseReportDto>> Handle(GetDepartmentCourseReportQuery request, CancellationToken cancellationToken)
        {
            var department = await _unitOfWork.DepartmentRepository.GetFlexibleAsync(asNoTracking: true);
            var user = await _unitOfWork.UserRepository.GetFlexibleAsync(
                predicate: u => !u.IsDeleted,
                asNoTracking: true
            );
            var userCourses = await _unitOfWork.UserCourseRepository.GetFlexibleAsync(asNoTracking: true);

            if (!department.Any())
                return new List<DepartmentCourseReportDto>();

            var result = department
                .GroupJoin(user,
                    d => d.DepartmentId,
                    u => u.DepartmentId,
                    (d, userGroup) => new
                    {
                        Department = d,
                        Users = userGroup
                    })
                .SelectMany(
                    d => d.Users.DefaultIfEmpty(),
                    (d, u) => new
                    {
                        d.Department,
                        User = u
                    })
                .GroupJoin(userCourses,
                    d => d.User != null ? d.User.Id : null,
                    uc => uc.UserId,
                    (d, ucGroup) => new
                    {
                        d.Department,
                        UserCourse = ucGroup
                    })
                .GroupBy(x => x.Department.DepartmentId)
                .Select(g => new DepartmentCourseReportDto
                {
                    DepartmentName = g.First().Department.DepartmentName ?? "Unknown",
                    NumberOfCourseParticipated = g.SelectMany(x => x.UserCourse).Select(uc => uc.CourseId).Distinct().Count(),
                    NumberOfLearnersCompleted = g.SelectMany(x => x.UserCourse).Where(uc => uc.Status == "Assigned").Select(uc => uc.UserId).Distinct().Count()
                })
                .OrderByDescending(dto => dto.NumberOfLearnersCompleted)
                .ToList();

            return result;
        }
    }
}
