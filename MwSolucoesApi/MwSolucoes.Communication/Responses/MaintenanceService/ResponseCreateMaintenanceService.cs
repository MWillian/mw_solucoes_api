using MwSolucoes.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace MwSolucoes.Communication.Responses.MaintenanceService
{
    public class ResponseCreateMaintenanceService
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public MaintenanceServiceCategories Category { get; set; }
    }
}
