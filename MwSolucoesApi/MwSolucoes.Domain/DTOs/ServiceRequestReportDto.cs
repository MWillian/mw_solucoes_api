using MwSolucoes.Domain.Enums;

namespace MwSolucoes.Domain.DTOs
{
    public record ServiceRequestReportDto
    {
        public string Protocol { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerCpf { get; set; } = string.Empty;
        public string CustomerPhone { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        public EquipmentType Equipment { get; set; }
        public string BrandModel { get; set; } = string.Empty;
        public string ReportedProblem { get; set; } = string.Empty;
        public string TechnicalDiagnosis { get; set; } = string.Empty;
        public decimal? LaborCost { get; set; }
        public decimal? PartsCost { get; set; }
        public ServiceRequestStatus Status { get; set; }
        public List<MaintenanceServiceItemDto> Services { get; set; } = [];
    }
    public record MaintenanceServiceItemDto
    {
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
    }
    public record ReceiptReportDto
    {
        public string Protocol { get; set; } = string.Empty;
        public DateTime FinishedAt { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerCpf { get; set; } = string.Empty;
        public EquipmentType Equipment { get; set; }
        public string BrandModel { get; set; } = string.Empty;
        public decimal? LaborCost { get; set; }
        public decimal? PartsCost { get; set; }
        public List<MaintenanceServiceItemDto> Services { get; set; } = [];
        public decimal? TotalAmount { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public DateTime WarrantyEndDate => FinishedAt.AddDays(90);
    }
}
