namespace QLDT_Becamex.Src.Dtos.Departments
{
    public class DepartmentDto
    {
        public string? DepartmentId { get; set; }
        public string? DepartmentName { get; set; }
        public string? DepartmentCode { get; set; }
        public string? Description { get; set; }
        public string? ParentId { get; set; }
        public string? ParentName { get; set; }
        public string? ManagerId { get; set; }
        public string? ManagerName { get; set; }
        public string? Status { get; set; }
        public int Level { get; set; }
        public List<string>? Path { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
