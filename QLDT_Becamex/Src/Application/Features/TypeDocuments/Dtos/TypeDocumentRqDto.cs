using System.ComponentModel.DataAnnotations;

namespace QLDT_Becamex.Src.Application.Features.TypeDocument.Dtos
{
    public class TypeDocumentRqDto
    {
        [Required(ErrorMessage = "TypeName is required")]
        public string? NameType { get; set; }
    }
}
