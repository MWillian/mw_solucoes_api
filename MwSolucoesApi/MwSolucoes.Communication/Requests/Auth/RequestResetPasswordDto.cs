using System.ComponentModel.DataAnnotations;

namespace MwSolucoes.Communication.Requests.Auth
{
    public class RequestResetPasswordDto
    {
        [Required(ErrorMessage = "O token de segurança é obrigatório.")]
        public string Token { get; set; } = string.Empty;

        [Required(ErrorMessage = "A nova senha é obrigatória.")]
        [MinLength(6, ErrorMessage = "A nova senha deve ter no mínimo 6 caracteres.")]
        public string NewPassword { get; set; } = string.Empty;
    }
}
