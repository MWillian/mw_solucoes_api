using MwSolucoes.Exception.ResouceErrors;
using MwSolucoes.Exception.ResouceErrors.DomainErrorMessages;
using MwSolucoes.Exception.ResouceErrors.UseCaseErrorMessages;
using System.ComponentModel.DataAnnotations;

namespace MwSolucoes.Communication.Requests
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
        [StringLength(11, MinimumLength = 11, ErrorMessageResourceType = typeof(UserErrorMessages), ErrorMessageResourceName = "INVALID_CPF")]
        public string Cpf { get; set; } = string.Empty;

        [Required(ErrorMessageResourceType = typeof(UserErrorMessages), ErrorMessageResourceName = "EMPTY_ROLE")]
        public string Role { get; set; } = string.Empty;

        [Required(ErrorMessageResourceType = typeof(UserErrorMessages), ErrorMessageResourceName = "EMPTY_PASSWORD")]
        [StringLength(40, MinimumLength = 8, ErrorMessageResourceType = typeof(UserErrorMessages), ErrorMessageResourceName = "INVALID_PASSWORD")]
        public string Password { get; set; } = string.Empty;

        [Compare("Password", ErrorMessageResourceType = typeof(RegisterUserErrorMessages), ErrorMessageResourceName = "DIFERENT_PASSWORDS")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
