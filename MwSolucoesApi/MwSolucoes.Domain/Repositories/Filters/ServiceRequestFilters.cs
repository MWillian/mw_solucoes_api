using MwSolucoes.Domain.Enums;

namespace MwSolucoes.Domain.Repositories.Filters
{
    public class ServiceRequestFilters
    {
        public ServiceRequestStatus? Status { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string? Protocol { get; set; }
        public EquipmentType? EquipmentType { get; set; }
        public decimal? LaborCost { get; set; }
        public decimal? PartsCost { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public string? SortBy { get; set; } = "createdat";
        public string? SortDirection { get; set; } = "desc";
    }
}
