using MwSolucoes.Communication.Requests;
using MwSolucoes.Communication.Responses.Login;

namespace MwSolucoes.Application.UseCases.Auth
{
    public interface ILoginUseCase
    {
        public Task<ResponseLogin> Execute(RequestLogin request);
    }
}
