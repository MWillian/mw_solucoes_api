using MwSolucoes.Communication.Requests.Auth;
using MwSolucoes.Communication.Requests.Login;
using MwSolucoes.Communication.Responses.Login;

namespace MwSolucoes.Application.Interfaces
{
    public interface IAuthService
    {
        public Task<ResponseLogin> LoginAsync(RequestLogin request);
        public Task UpdatePasswordAsync(Guid userId, RequestUpdatePassword request);
    }
}
