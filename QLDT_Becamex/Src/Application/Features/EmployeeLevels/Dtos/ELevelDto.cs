using System.ComponentModel.DataAnnotations;

namespace QLDT_Becamex.Src.Application.Features.EmployeeLevels.Dtos
{
    public class ELevelDto
    {
        public int ELevelId { get; set; } // Khóa chính
        public string? ELevelName { get; set; }
    }
    public class CreateELevelDto
    {
        [Required]
        public string ELevelName { get; set; } = null!;
    }
}
