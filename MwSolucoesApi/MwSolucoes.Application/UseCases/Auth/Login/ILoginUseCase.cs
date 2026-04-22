using MwSolucoes.Communication.Requests.Login;
using MwSolucoes.Communication.Responses.Login;

namespace MwSolucoes.Application.UseCases.Auth.Login
{
    public interface ILoginUseCase
    {
        public Task<ResponseLogin> Execute(RequestLogin request);
    }
}
