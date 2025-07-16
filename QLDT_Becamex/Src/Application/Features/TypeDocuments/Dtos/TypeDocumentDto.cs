namespace QLDT_Becamex.Src.Application.Features.TypeDocument.Dtos
{
    public class TypeDocumentDto
    {
        public int Id { get; set; }
        public string? NameType { get; set; }
        public int Key { get; set; } // 0: PDF, 1: Link
    }
}
