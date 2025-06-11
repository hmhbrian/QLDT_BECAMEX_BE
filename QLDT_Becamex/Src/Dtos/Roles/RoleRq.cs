using System.ComponentModel.DataAnnotations;

namespace QLDT_Becamex.Src.Dtos.Roles
{
    public class RoleRq
    {
        [Required]
        public string RoleName { get; set; } = null!;

    }
}
