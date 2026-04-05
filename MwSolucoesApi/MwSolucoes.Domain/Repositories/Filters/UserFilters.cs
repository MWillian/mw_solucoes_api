namespace MwSolucoes.Domain.Repositories.Filters
{
    public class UserFilters
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public bool? IsActive { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public string? SortBy { get; set; } = "name";
        public string? SortDirection { get; set; } = "asc";
    }
}
