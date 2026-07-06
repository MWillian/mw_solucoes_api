using MwSolucoes.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace MwSolucoes.Communication.Requests.ServiceRequestHistory
{
    public class RequestUpdateServiceRequestHistory
    {
        [Required(ErrorMessage = "Novo status da solicitação vazio.")]
        public ServiceRequestHistoryStatus Status { get; private set; }
        [Required(ErrorMessage = "Descrição da solicitação vazia.")]
        public string Description { get; private set; } = string.Empty;
    }
}
