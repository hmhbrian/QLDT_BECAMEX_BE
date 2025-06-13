using QLDT_Becamex.Src.Dtos.Params;

namespace QLDT_Becamex.Src.Dtos.Results
{
    public class PagedResult<T>
    {
        public IEnumerable<T>? Items { get; set; } = new List<T>();
        public Pagination? Pagination { get; set; }
    }
}
