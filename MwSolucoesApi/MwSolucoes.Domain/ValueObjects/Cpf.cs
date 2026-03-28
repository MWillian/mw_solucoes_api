using MwSolucoes.Exception.ExceptionBase;
using MwSolucoes.Exception.ResouceErrors.DomainErrorMessages;
using System.Text.RegularExpressions;

namespace MwSolucoes.Domain.ValueObjects
{
    public sealed record Cpf
    {
        public string Number { get; }

        public Cpf(string number)
        {
            if (string.IsNullOrWhiteSpace(number))
                throw new DomainException(UserErrorMessages.EMPTY_CPF);

            var cleanedNumber = Clear(number);

            Validate(cleanedNumber);

            Number = cleanedNumber;
        }

        private static string Clear(string number)
        {
            return Regex.Replace(number, @"[^\d]", "");
        }

        private static void Validate(string cpf)
        {
            if (cpf.Length != 11)
                throw new DomainException(UserErrorMessages.INVALID_CPF);

            if (HaveAllSameDigits(cpf))
                throw new DomainException(UserErrorMessages.INVALID_CPF);

            if (!ValidateDigits(cpf))
                throw new DomainException(UserErrorMessages.INVALID_CPF);
        }

        private static bool HaveAllSameDigits(string cpf)
        {
            return cpf switch
            {
                "00000000000" => true,
                "11111111111" => true,
                "22222222222" => true,
                "33333333333" => true,
                "44444444444" => true,
                "55555555555" => true,
                "66666666666" => true,
                "77777777777" => true,
                "88888888888" => true,
                "99999999999" => true,
                _ => false
            };
        }

        private static bool ValidateDigits(string cpf)
        {
            var firstMultiplier = new int[9] { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            var secondMultiplier = new int[10] { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };

            var tempCpf = cpf.Substring(0, 9);
            var sum = 0;

            for (var i = 0; i < 9; i++)
                sum += int.Parse(tempCpf[i].ToString()) * firstMultiplier[i];

            var rest = sum % 11;
            if (rest < 2)
                rest = 0;
            else
                rest = 11 - rest;

            var digit = rest.ToString();
            tempCpf += digit;
            sum = 0;

            for (var i = 0; i < 10; i++)
                sum += int.Parse(tempCpf[i].ToString()) * secondMultiplier[i];

            rest = sum % 11;
            if (rest < 2)
                rest = 0;
            else
                rest = 11 - rest;

            digit += rest.ToString();

            return cpf.EndsWith(digit);
        }

        public string FormatarParaSaida()
        {
            return long.Parse(Number).ToString(@"000\.000\.000\-00");
        }

        public override string ToString() => Number;

        public static implicit operator string(Cpf cpf) => cpf.Number;
    }
}