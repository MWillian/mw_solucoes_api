using MwSolucoes.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace MwSolucoes.Communication.Requests.ServiceRequest
{
    public class RequestUpdateServiceRequest
    {
        [Required(ErrorMessage = "Status da solicitação é obrigatório.")]
        public ServiceRequestStatus Status { get; set; }
        public string? TechnicalDiagnosis { get; set; }
        [Range(0, double.MaxValue, ErrorMessage = "Valor de mão de obra não pode ser negativo.")]
        public decimal? LaborCost { get; set; }
        [Range(0, double.MaxValue, ErrorMessage = "Valor de peças não pode ser negativo.")]
        public decimal? PartsCost { get; set; }
    }
}
