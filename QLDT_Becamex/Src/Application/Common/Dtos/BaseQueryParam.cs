using System.ComponentModel.DataAnnotations;

namespace QLDT_Becamex.Src.Application.Common.Dtos
{
    public class BaseQueryParam
    {
        [Range(1, int.MaxValue, ErrorMessage = "Page must be a positive integer.")]
        public int Page { get; set; } = 1;
        [Range(1, 24, ErrorMessage = "Limit must be between 1 and 24.")]
        public int Limit { get; set; } = 24;
        public string SortField { get; set; } = "created.at";

        [RegularExpression("^(asc|desc)$", ErrorMessage = "SortType must be 'asc' or 'desc'.")]
        public string SortType { get; set; } = "desc";
    }

    public class Pagination
    {
        public int? TotalItems { get; set; }
        public int? ItemsPerPage { get; set; }
        public int? CurrentPage { get; set; }
        public int? TotalPages { get; set; }
    }

    public class BaseQueryParamFilter : BaseQueryParam
    {
        public string? Keyword { get; set; }
        public string? StatusIds { get; set; }
        public string? DepartmentIds { get; set; }
        public string? PositionIds { get; set; }
        public string? CategoryIds { get; set; }
        public string? LecturerIds { get; set; }
        public string? FromDate { get; set; }
        public string? ToDate { get; set; }
    }
}
