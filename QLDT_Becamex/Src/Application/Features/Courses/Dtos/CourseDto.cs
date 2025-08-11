
using QLDT_Becamex.Src.Application.Features.CourseCategory.Dtos;
using QLDT_Becamex.Src.Application.Features.Departments.Dtos;
using QLDT_Becamex.Src.Application.Features.EmployeeLevels.Dtos;
using QLDT_Becamex.Src.Application.Features.Status.Dtos;
using QLDT_Becamex.Src.Application.Features.Users.Dtos;
using QLDT_Becamex.Src.Domain.Entities;


namespace QLDT_Becamex.Src.Application.Features.Courses.Dtos
{
    public class CourseDto
    {
        public string Id { get; set; } = null!;
        public string? Code { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Objectives { get; set; }
        public string? ThumbUrl { get; set; }
        public string? Format { get; set; }
        public int? Sessions { get; set; }
        public int? HoursPerSessions { get; set; }
        public string? Optional { get; set; }
        public int? MaxParticipant { get; set; }
        public UserSumaryDto? CreatedBy { get; set; }
        public UserSumaryDto? UpdatedBy { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? RegistrationStartDate { get; set; }
        public DateTime? RegistrationClosingDate { get; set; }
        public string? Location { get; set; }
        public bool? IsPrivate { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public StatusDto? Status { get; set; }
        public CourseCategoryDto? Category { get; set; }
        public ICollection<UserSumaryDto>? Students { get; set; } = new List<UserSumaryDto>();
        public ICollection<DepartmentShortenDto>? Departments { get; set; } = new List<DepartmentShortenDto>();
        public ICollection<ELevelDto>? ELevels { get; set; } = new List<ELevelDto>();
    }
}