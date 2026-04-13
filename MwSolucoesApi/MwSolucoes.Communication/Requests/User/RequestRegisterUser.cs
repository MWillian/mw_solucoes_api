using MwSolucoes.Exception.ResouceErrors;
using MwSolucoes.Exception.ResouceErrors.DomainErrorMessages;
using MwSolucoes.Exception.ResouceErrors.UseCaseErrorMessages;
using System.ComponentModel.DataAnnotations;

namespace MwSolucoes.Communication.Requests.User
{
    public class RequestRegisterUser
    {
        [Required(ErrorMessageResourceType = typeof(UserErrorMessages), ErrorMessageResourceName = "EMPTY_USERNAME")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessageResourceType = typeof(UserErrorMessages), ErrorMessageResourceName = "EMPTY_EMAIL")]
        [EmailAddress(ErrorMessageResourceType = typeof(UserErrorMessages), ErrorMessageResourceName = "INVALID_EMAIL")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessageResourceType = typeof(UserErrorMessages), ErrorMessageResourceName = "EMPTY_PHONE_NUMBER")]
        [StringLength(11, MinimumLength = 11, ErrorMessageResourceType = typeof(UserErrorMessages), ErrorMessageResourceName = "INVALID_PHONE_NUMBER")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required(ErrorMessageResourceType = typeof(UserErrorMessages), ErrorMessageResourceName = "EMPTY_CPF")]
        [StringLength(14, MinimumLength = 11, ErrorMessageResourceType = typeof(UserErrorMessages), ErrorMessageResourceName = "INVALID_CPF")]
        public string Cpf { get; set; } = string.Empty;

        [Required(ErrorMessageResourceType = typeof(UserErrorMessages), ErrorMessageResourceName = "EMPTY_ROLE")]
        public int Role { get; set; }

        [Required(ErrorMessageResourceType = typeof(UserErrorMessages), ErrorMessageResourceName = "EMPTY_PASSWORD")]
        [StringLength(40, MinimumLength = 8, ErrorMessageResourceType = typeof(UserErrorMessages), ErrorMessageResourceName = "INVALID_PASSWORD")]
        public string Password { get; set; } = string.Empty;

        [Compare("Password", ErrorMessageResourceType = typeof(RegisterUserErrorMessages), ErrorMessageResourceName = "DIFERENT_PASSWORDS")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required]
        public string Logradouro { get; set; } = string.Empty;

        [Required(ErrorMessage = "Nivel de acesso vazio.")]
        public int AccessLevel { get; set; }

        [Required]
        public string Numero { get; set; } = string.Empty;

        [Required]
        public string Bairro { get; set; } = string.Empty;

        [Required]
        public string Cidade { get; set; } = string.Empty;

        [Required]
        [StringLength(2, MinimumLength = 2)]
        public string Estado { get; set; } = string.Empty;

        [Required]
        [StringLength(9, MinimumLength = 8)]
        public string Cep { get; set; } = string.Empty;
    }
}
