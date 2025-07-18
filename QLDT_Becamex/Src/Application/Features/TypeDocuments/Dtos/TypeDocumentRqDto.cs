using System.ComponentModel.DataAnnotations;

namespace QLDT_Becamex.Src.Application.Features.TypeDocument.Dtos
{
    public class TypeDocumentRqDto
    {
        [Required(ErrorMessage = "TypeName is required")]
        public string? NameType { get; set; }
        [Required(ErrorMessage = "Key is required")]
        public int Key { get; set; } = 0; // 0 là PDF, 1 là link 
    }
}
