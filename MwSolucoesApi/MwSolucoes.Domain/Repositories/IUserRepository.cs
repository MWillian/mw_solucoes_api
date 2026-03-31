using MwSolucoes.Domain.Entities;

namespace MwSolucoes.Domain.Repositories
{
    public interface IUserRepository
    {
        Task Add(User user);
        Task<User?> GetByEmail(string email);
        Task<User?> GetById(Guid id);
        Task Update(User user);
        Task<bool> ExistUserWithEmail(string email);
        Task<bool> ExistUserWithPhoneNumber(string phoneNumber);
    }
}
