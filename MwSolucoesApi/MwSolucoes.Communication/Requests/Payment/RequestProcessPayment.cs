using MwSolucoes.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace MwSolucoes.Communication.Requests.Payment
{
    public class RequestProcessPayment
    {
        [Required(ErrorMessage = "O método de pagamento é obrigatório.")]
        public PaymentMethod PaymentMethod { get; set; }
    }
}
