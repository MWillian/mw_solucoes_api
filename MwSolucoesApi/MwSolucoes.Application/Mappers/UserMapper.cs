using MwSolucoes.Communication.Requests;
using MwSolucoes.Communication.Responses.User;
using MwSolucoes.Domain.Entities;

namespace MwSolucoes.Application.Mappers
{
    public static class UserMapper
    {
        public static ResponseRegisterUser ToResponseRegisterUser(User user)
        {
            return new ResponseRegisterUser
            {
                Name = user.Name,
            };
        }
        public static User ToUser(RequestRegisterUser request, string passwordHash)
        {
            return new User(request.Name, request.Email, passwordHash, request.PhoneNumber, request.Cpf, request.Role);
        }
    }
}
