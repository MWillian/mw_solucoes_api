using MwSolucoes.Communication.Requests;
using MwSolucoes.Communication.Responses.User;
using MwSolucoes.Domain.Entities;
using MwSolucoes.Domain.Security.Tokens;
using MwSolucoes.Domain.ValueObjects;

namespace MwSolucoes.Application.Mappers
{
    public static class UserMapper
    {
        public static ResponseRegisterUser ToResponseRegisterUser(User user, ITokenGenerator generatedToken)
        {
            return new ResponseRegisterUser
            {
                Name = user.Name,
                Token = generatedToken.GenerateToken(user)
            };
        }
        public static User ToUser(RequestRegisterUser request, string passwordHash)
        {
            var address = new Address(request.Logradouro, request.Numero, request.Bairro, request.Cidade, request.Estado, request.Cep);
            return new User(request.Name, request.Email, passwordHash, request.PhoneNumber, request.Cpf, request.Role, address);
        }
    }
}
