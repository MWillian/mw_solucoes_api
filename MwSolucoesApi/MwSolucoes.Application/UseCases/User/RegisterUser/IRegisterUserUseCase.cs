using MwSolucoes.Communication.Requests.User;
using MwSolucoes.Communication.Responses.User;

namespace MwSolucoes.Application.UseCases.User.RegisterUser
{
    public interface IRegisterUserUseCase
    {
        Task<ResponseRegisterUser> Execute(RequestRegisterUser request);
    }
}
