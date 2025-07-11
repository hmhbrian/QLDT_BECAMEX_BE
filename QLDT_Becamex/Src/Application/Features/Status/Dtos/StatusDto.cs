using System.ComponentModel.DataAnnotations;

namespace QLDT_Becamex.Src.Application.Features.Status.Dtos
{
    public class StatusDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
    }

    public class CreateStatusDto
    {
        [Required]
        public string Name { get; set; } = null!;
    }
}
