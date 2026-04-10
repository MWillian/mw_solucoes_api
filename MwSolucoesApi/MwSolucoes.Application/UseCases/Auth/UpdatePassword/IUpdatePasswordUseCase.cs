using MwSolucoes.Communication.Requests;

namespace MwSolucoes.Application.UseCases.Auth.UpdatePassword
{
    public interface IUpdatePasswordUseCase
    {
        Task Execute(Guid userId, RequestUpdatePassword request);
    }
}