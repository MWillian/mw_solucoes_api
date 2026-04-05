using MwSolucoes.Communication.Responses.User;

namespace MwSolucoes.Application.UseCases.User.GetUser
{
    public interface IGetUserByIdUseCase
    {
        Task<ResponseGetUser> Execute(Guid id);
    }
}
