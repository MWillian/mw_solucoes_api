using System.ComponentModel.DataAnnotations;

namespace MwSolucoes.Communication.Requests.Auth
{
    public class RequestForgotPassword
    {
        [Required(ErrorMessage = "Email inválido.")]
        [EmailAddress(ErrorMessage = "Insira um formato de e-mail válido.")]
        public string Email { get; set; } = string.Empty;
    }
}
