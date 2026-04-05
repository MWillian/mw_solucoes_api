using MwSolucoes.Communication.Requests;
using MwSolucoes.Communication.Responses.User;

namespace MwSolucoes.Application.UseCases.User.UpdateUser
{
    public interface IUpdateUserUseCase
    {
        Task<ResponseUpdateUser> Execute(Guid Id, RequestUpdateUser request);
    }
}
