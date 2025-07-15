using QLDT_Becamex.Src.Application.Features.Status.Dtos;

namespace QLDT_Becamex.Src.Application.Features.Departments.Dtos
{
    public class DepartmentDto
    {
        public int DepartmentId { get; set; }
        public string? DepartmentName { get; set; }
        public string? DepartmentCode { get; set; }
        public string? Description { get; set; }
        public int? ParentId { get; set; }
        public string? ParentName { get; set; }
        public string? ManagerId { get; set; }
        public string? ManagerName { get; set; }
        public StatusDto? Status { get; set; }
        public int Level { get; set; }
        public List<string>? Path { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<DepartmentDto>? Children { get; set; }
    }

    public class DepartmentShortenDto
    {
        public int DepartmentId { get; set; }
        public string? DepartmentName { get; set; }
    }
}
