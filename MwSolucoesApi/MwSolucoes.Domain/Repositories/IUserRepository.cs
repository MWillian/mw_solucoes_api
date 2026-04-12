using MwSolucoes.Domain.Entities;
using MwSolucoes.Domain.Repositories.Filters;

namespace MwSolucoes.Domain.Repositories
{
    public interface IUserRepository
    {
        Task Add(User user);
        Task<User?> GetByEmail(string email);
        Task<User?> GetById(Guid id);
        Task<PagedResult<User>> GetAll(UserFilters filters);
        Task Update(User user);
        Task<bool> ExistUserWithEmail(string email);
        Task<bool> ExistUserWithPhoneNumber(string phoneNumber);
        Task<bool> ExistUserByCpf(string cpf);
        Task<bool> IsActive(Guid id);
    }
}
