using MwSolucoes.Communication.Requests;

namespace MwSolucoes.Application.UseCases.User.UpdateUser
{
    public interface IUpdateUserUseCase
    {
        Task Execute(RequestUpdateUser request);
    }
}
