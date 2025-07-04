

namespace QLDT_Becamex.Src.Application.Common.Dtos
{
    public class PagedResult<T>
    {
        public IEnumerable<T>? Items { get; set; } = new List<T>();
        public Pagination? Pagination { get; set; }

        public PagedResult(IEnumerable<T> items, Pagination pagination)
        {
            Items = items;
            Pagination = pagination;
        }
    }
}
