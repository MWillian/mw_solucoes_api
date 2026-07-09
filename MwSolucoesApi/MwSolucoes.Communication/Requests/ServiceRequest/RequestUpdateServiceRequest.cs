using System.ComponentModel.DataAnnotations;

namespace MwSolucoes.Communication.Requests.ServiceRequest
{
    public class RequestUpdateServiceRequest
    {
        public string TechnicalDiagnosis { get; set; } = string.Empty;
        [Range(0, double.MaxValue, ErrorMessage = "Valor de peças não pode ser negativo.")]
        public decimal? PartsCost { get; set; }

        [Required(ErrorMessage = "Para gerar o orçamento, insira ao menos um serviço.")]
        [MinLength(1, ErrorMessage = "Selecione ao menos um serviço no catálogo.")]
        public List<int> ServiceIds { get; set; } = [];
    }
}
