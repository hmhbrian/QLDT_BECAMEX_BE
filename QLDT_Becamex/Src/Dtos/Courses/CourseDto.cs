using QLDT_Becamex.Src.Constant;
using QLDT_Becamex.Src.Dtos.Departments;
using QLDT_Becamex.Src.Dtos.Positions;
using QLDT_Becamex.Src.Models;
using System.ComponentModel.DataAnnotations;

namespace QLDT_Becamex.Src.Dtos.Courses
{
    public class CourseDto
    {
        public string Id { get; set; } = null!;
        public string? Code { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Objecttives { get; set; }
        public string? ThumbUrl { get; set; }
        public string? Format { get; set; }
        public int? Sesstions { get; set; }
        public int? HoursPerSesstions { get; set; }
        public string? Optional { get; set; }
        public int? MaxParticipant { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? RegistrationStartDate { get; set; }
        public DateTime? RegistrationSlosingDate { get; set; }
        public string? Location { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }

        public int? StatusId { get; set; }
        public CourseSatus? Status { get; set; }
        public ICollection<DepartmentDto>? Departments { get; set; } = new List<DepartmentDto>();
        public ICollection<PositionDto>? Positions { get; set; } = new List<PositionDto>();
    }

    
}
