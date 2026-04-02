using MwSolucoes.Exception.ExceptionBase;

namespace MwSolucoes.Domain.ValueObjects
{
    public class Address
    {
        public string Logradouro { get; private set; } = string.Empty;
        public string Numero { get; private set; } = string.Empty;
        public string Bairro { get; private set; } = string.Empty;
        public string Cidade { get; private set; } = string.Empty;
        public string Estado { get; private set; } = string.Empty;
        public string Cep { get; private set; } = string.Empty;

        private Address() { }

        public Address(string logradouro, string numero, string bairro, string cidade, string estado, string cep)
        {
            Validate(logradouro, numero, bairro, cidade, estado, cep);
            Logradouro = logradouro;
            Numero = numero;
            Bairro = bairro;
            Cidade = cidade;
            Estado = estado;
            Cep = ClearCep(cep);
        }

        private static void Validate(string logradouro, string numero, string bairro, string cidade, string estado, string cep)
        {
            if (string.IsNullOrWhiteSpace(logradouro))
                throw new DomainException("Logradouro é obrigatório.");

            if (string.IsNullOrWhiteSpace(numero))
                throw new DomainException("Número é obrigatório.");

            if (string.IsNullOrWhiteSpace(bairro))
                throw new DomainException("Bairro é obrigatório.");

            if (string.IsNullOrWhiteSpace(cidade))
                throw new DomainException("Cidade é obrigatória.");

            if (string.IsNullOrWhiteSpace(estado))
                throw new DomainException("Estado é obrigatório.");

            var clearCep = ClearCep(cep);
            if (clearCep.Length != 8)
                throw new DomainException("CEP inválido.");
        }

        private static string ClearCep(string cep)
        {
            return new string(cep.Where(char.IsDigit).ToArray());
        }
    }
}
