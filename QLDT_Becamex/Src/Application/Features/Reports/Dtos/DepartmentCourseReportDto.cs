namespace QLDT_Becamex.Src.Application.Features.Reports.Dtos
{
    public class DepartmentCourseReportDto
    {
        public string? DepartmentName { get; set; }
        public int NumberOfUsersParticipated { get; set; }
        public int TotalUsers { get; set; }
        public double ParticipationRate { get; set; }
    }
}
