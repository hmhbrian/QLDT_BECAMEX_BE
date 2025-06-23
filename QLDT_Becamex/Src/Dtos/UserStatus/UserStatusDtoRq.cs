using System.ComponentModel.DataAnnotations;

namespace QLDT_Becamex.Src.Dtos.UserStatus
{
    public class UserStatusDtoRq
    {
        [Required]
        public string Name { get; set; } = null!;
    }
}
