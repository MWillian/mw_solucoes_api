using MwSolucoes.Communication.Requests.User;
using MwSolucoes.Communication.Responses;
using MwSolucoes.Communication.Responses.User;

namespace MwSolucoes.Application.UseCases.User.GetUsers
{
    public interface IGetUsersUseCase
    {   
        Task<PagedResult<ResponseGetUser>> Execute(UserFilters filters);
    }
}
