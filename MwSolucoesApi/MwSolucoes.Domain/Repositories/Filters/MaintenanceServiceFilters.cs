using MwSolucoes.Domain.Enums;

namespace MwSolucoes.Domain.Repositories.Filters
{
    public class MaintenanceServiceFilters
    {
        public string? Name { get; set; }
        public decimal? Price { get; set; }
        public bool? IsActive { get; set; }
        public MaintenanceServiceCategories? Category { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public string? SortBy { get; set; } = "name";
        public string? SortDirection { get; set; } = "asc";
    }
}
