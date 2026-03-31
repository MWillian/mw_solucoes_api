using MwSolucoes.Communication.Requests;
using MwSolucoes.Exception.ExceptionBase;
using System.ComponentModel.DataAnnotations;

namespace MwSolucoes.Application.UseCases.User.RegisterUser
{
    public static class RegisterUserValidator
    {
        public static void Validate(RequestRegisterUser request)
        {
            var context = new ValidationContext(request);
            List<ValidationResult> validationResults = [];
            bool isValid = Validator.TryValidateObject(request, context, validationResults, true);

            if (!isValid)
            {
                var errorMessages = validationResults
                    .Select(vr => vr.ErrorMessage)
                    .Where(em => !string.IsNullOrEmpty(em))
                    .Cast<string>()
                    .ToList();
                throw new ErrorOnValidationException(errorMessages);
            }
        }
    }
}
