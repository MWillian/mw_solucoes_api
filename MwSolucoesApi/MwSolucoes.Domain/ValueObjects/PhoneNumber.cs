using MwSolucoes.Exception.ExceptionBase;
using MwSolucoes.Exception.ResouceErrors.DomainErrorMessages;
using System.Text.RegularExpressions;

namespace MwSolucoes.Domain.ValueObjects
{
    public sealed record PhoneNumber
    {
        public string Number { get; } = string.Empty;
        public PhoneNumber(string phoneNumer)
        {
            if (string.IsNullOrWhiteSpace(phoneNumer))
            {
                throw new DomainException(UserErrorMessages.EMPTY_PHONE_NUMBER);
            }
            var clearedPhoneNumber = Clear(phoneNumer);
            ValidateLength(clearedPhoneNumber);
            Number = clearedPhoneNumber;
        }
        private static string Clear(string number)
        {
            return Regex.Replace(number, @"[^\d]", "");
        }

        private void ValidateLength(string phoneNumber)
        {
            if (phoneNumber.Length != 11)
            {
                throw new DomainException(UserErrorMessages.INVALID_PHONE_NUMBER);
            }
        }
        public static implicit operator string(PhoneNumber phoneNumber) => phoneNumber.Number;
    }
}
