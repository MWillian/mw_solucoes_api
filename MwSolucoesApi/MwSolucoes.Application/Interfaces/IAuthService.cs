using MwSolucoes.Communication.Requests.Auth;
using MwSolucoes.Communication.Requests.Login;
using MwSolucoes.Communication.Responses.Login;

namespace MwSolucoes.Application.Interfaces
{
    public interface IAuthService
    {
        Task<ResponseLogin> LoginAsync(RequestLogin request);
        Task UpdatePasswordMeAsync(Guid userId, RequestUpdatePassword request);
        Task ForgotPasswordAsync(string email);
        Task LogoutAsync(string cookieToken);
        Task<ResponseLogin> TokenRefresh(string cookieToken);
        Task ResetPasswordAsync(RequestResetPasswordDto request);
    }
}
