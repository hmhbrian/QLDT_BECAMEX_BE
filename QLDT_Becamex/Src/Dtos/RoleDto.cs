using System.ComponentModel.DataAnnotations;

namespace QLDT_Becamex.Src.Dtos
{
    public class RoleDto
    {
        public string Id { get; set; } = null!;

        public string Name { get; set; } = null!;
    }

    public class RoleRq
    {
        [Required]
        public string RoleName { get; set; } = null!;

    }
}
