using MwSolucoes.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace MwSolucoes.Communication.Requests.ServiceRequest
{
    public class RequestCreateServiceRequest
    {
        [Required(ErrorMessage = "Tipo do equipamento está vazio.")]
        public EquipmentType EquipmentType { get; set; }
        [Required(ErrorMessage = "A marca/modelo do dispositivo está vazio.")]
        [StringLength(100, ErrorMessage = "Marca/Modelo deve ter no máximo 100 caracteres.")]
        public string BrandModel { get; set; } = string.Empty;
        [Required(ErrorMessage = "A descrição inicial do problema do dispositivo está vazia.")]
        public string ReportedProblem { get; set; } = string.Empty;
        public string? TechnicalDiagnosis { get; set; }
        [Range(0, double.MaxValue, ErrorMessage = "Valor de mão de obra não pode ser negativo.")]
        public decimal LaborCost { get; set; }
        [Range(0, double.MaxValue, ErrorMessage = "Valor de peças não pode ser negativo.")]
        public decimal PartsCost { get; set; }
        public bool RequiresDownPayment { get; set; }
    }
}
