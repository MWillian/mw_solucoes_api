using MwSolucoes.Exception.ResouceErrors.UseCaseErrorMessages;
using System.ComponentModel.DataAnnotations;

namespace MwSolucoes.Communication.Requests.Auth
{
    public class RequestUpdatePassword
    {
        [Required(ErrorMessage = "Senha atual inválida.")]
        public string CurrentPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Nova senha inválida.")]
        public string NewPassword { get; set; } = string.Empty;

        [Required]
        [Compare("Password", ErrorMessageResourceType = typeof(RegisterUserErrorMessages), ErrorMessageResourceName = "DIFERENT_PASSWORDS")]
        public string ConfirmNewPassword { get; set; } = string.Empty;
    }
}
