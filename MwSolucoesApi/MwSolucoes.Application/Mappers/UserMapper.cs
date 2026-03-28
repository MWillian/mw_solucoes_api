using MwSolucoes.Communication.Responses.User;
using MwSolucoes.Domain.Entities;

namespace MwSolucoes.Application.Mappers
{
    public static class UserMapper
    {
        public static RegisterUserResponse ToRegisterUserResponse(User user)
        {
            return new RegisterUserResponse
            {
                Name = user.Name,
            };
        }
        public static User ToUser(RequestRegisterUser request)
        {
            return new User(request.Name, request.Email, request.Password, request.PhoneNumber, request.Cpf, request.Role);
        }
    }
}
