using MediatR;
using QLDT_Becamex.Src.Application.Features.Reports.Dtos;
using QLDT_Becamex.Src.Application.Features.Reports.Queries;
using QLDT_Becamex.Src.Domain.Entities;
using QLDT_Becamex.Src.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

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
            // Lấy danh sách phòng ban cùng với Users và UserCourses
            var department = await _unitOfWork.DepartmentRepository.GetFlexibleAsync(
                includes: q => q
                    .Include(a => a.Users.Where(u => !u.IsDeleted))
                    .ThenInclude(t => t.UserCourse),
                asNoTracking: true) ?? new List<Department>();

            if (!department.Any())
                return new List<DepartmentCourseReportDto>();

            var result = department
                .Where(d => (d.Users?.Count() ?? 0) >= 1) // Lọc các phòng ban có ít nhất 2 nhân viên
                .Select(d => new DepartmentCourseReportDto
                {
                    DepartmentName = d.DepartmentName ?? "Unknown",
                    TotalUsers = d.Users?.Count() ?? 0, // Tổng số nhân viên trong phòng ban
                    NumberOfUsersParticipated = d.Users
                        ?.SelectMany(u => u.UserCourse ?? Enumerable.Empty<UserCourse>())
                        .Select(uc => uc.UserId)
                        .Distinct()
                        .Count() ?? 0, // Số người tham gia khóa học
                    ParticipationRate = (d.Users?.Count() ?? 0) > 0
                        ? (double)(d.Users?.SelectMany(u => u.UserCourse ?? Enumerable.Empty<UserCourse>())
                            .Select(uc => uc.UserId)
                            .Distinct()
                            .Count() ?? 0) / (d.Users?.Count() ?? 1)
                        : 0 // Tỷ lệ tham gia
                })
                .OrderByDescending(dto => dto.ParticipationRate) // Sắp xếp theo tỷ lệ tham gia
                .ThenByDescending(dto => dto.NumberOfUsersParticipated) // Sau đó theo số người tham gia
                .ToList();

            return result;
        }
    }
}
