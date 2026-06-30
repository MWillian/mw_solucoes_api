using MwSolucoes.Communication.Requests.User;
using MwSolucoes.Communication.Responses;
using MwSolucoes.Communication.Responses.User;

namespace MwSolucoes.Application.Interfaces
{
    public interface IUserService
    {
        Task<ResponseUpdateUser> UpdateUser(Guid id, RequestUpdateUser request);
        Task<ResponseRegisterUser> RegisterUser(RequestRegisterUser request);
        Task<PagedResult<ResponseGetUser>> GetUserList(UserFilters filters);
        Task DeactivateUser(Guid id);
        Task ActivateUser(Guid id);
        Task<ResponseGetUser> GetUserById(Guid id);
    }
}
