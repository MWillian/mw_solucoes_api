using MwSolucoes.Exception.ResouceErrors.DomainErrorMessages;
using System.ComponentModel.DataAnnotations;

namespace MwSolucoes.Communication.Requests
{
    public class RequestLogin
    {
        [Required(ErrorMessageResourceType = typeof(UserErrorMessages), ErrorMessageResourceName = "EMPTY_EMAIL")]
        [EmailAddress(ErrorMessageResourceType = typeof(UserErrorMessages), ErrorMessageResourceName = "INVALID_EMAIL")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessageResourceType = typeof(UserErrorMessages), ErrorMessageResourceName = "EMPTY_PASSWORD")]
        [StringLength(40, MinimumLength = 8, ErrorMessageResourceType = typeof(UserErrorMessages), ErrorMessageResourceName = "INVALID_PASSWORD")]
        public string Password { get; set; } = string.Empty;
    }
}
