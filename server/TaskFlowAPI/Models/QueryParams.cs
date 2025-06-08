namespace TaskFlowAPI.Models
{
    public class QueryParams
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string SortBy { get; set; } = "CreatedAtUtc";
        public bool SortDesc { get; set; } = true;
        public Dictionary<string, string> Filters { get; set; } = [];
    }

}
