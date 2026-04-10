using MwSolucoes.Communication.Requests;
using MwSolucoes.Communication.Responses;
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
        public static ResponseLogin ToResponseLogin(User user, ITokenGenerator generatedToken)
        {
            return new ResponseLogin
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
        public static ResponseGetUser ToResponseGetUser(User user)
        {
            return new ResponseGetUser
            {
                Id = user.Id,
                Name = user.Name,
                Address = user.Address,
                Cpf = user.CPF,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber
            };
        }
        public static Communication.Responses.PagedResult<ResponseGetUser> ToResponseGetUsers(Domain.Entities.PagedResult<User> users)
        {
            var responseItems = users.Items.Select(ToResponseGetUser).ToList();
            Communication.Responses.PagedResult<ResponseGetUser> result = new(responseItems, users.TotalCount, users.CurrentPage, users.PageSize);
            return result;
        }

        public static ResponseUpdateUser ToResponseUpdateUser(User user)
        {
            return new ResponseUpdateUser
            {
                Name = user.Name,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Cpf = user.CPF,
                Logradouro = user.Address.Logradouro,
                Numero = user.Address.Numero,
                Bairro = user.Address.Bairro,
                Cidade = user.Address.Cidade,
                Estado = user.Address.Estado,
                Cep = user.Address.Cep,
                Role = (int)user.Role
            };
        }
    }
}
