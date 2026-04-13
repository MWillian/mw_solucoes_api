using System.ComponentModel.DataAnnotations;

namespace MwSolucoes.Communication.Requests.Auth
{
    public class RequestUpdatePassword
    {
        [Required(ErrorMessage = "Senha atual inválida.")]
        public string CurrentPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Nova senha inválida.")]
        public string NewPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "A senha de confirmação e a nova senha devem ser iguais.")]
        public string ConfirmNewPassword { get; set; } = string.Empty;
    }
}
