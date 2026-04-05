using MwSolucoes.Domain.ValueObjects;

namespace MwSolucoes.Communication.Responses.User
{
    public class ResponseGetUser
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public PhoneNumber PhoneNumber { get; set; }
        public Address Address { get; set; }
        public Cpf Cpf { get; set; }
    }
}
