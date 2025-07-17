namespace QLDT_Becamex.Src.Domain.Entities
{
    public class TypeDocument
    {
        public int Id { get; set; }
        public int Key { get; set; } // Mã định danh duy nhất cho loại tài liệu
        public string NameType { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
    }
}
