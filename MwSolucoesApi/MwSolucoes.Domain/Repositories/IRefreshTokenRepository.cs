using MwSolucoes.Domain.Entities;

namespace MwSolucoes.Domain.Repositories
{
    public interface IRefreshTokenRepository
    {
        Task Add(RefreshToken refreshToken);
        Task Delete(string refreshToken);
        Task<RefreshToken?> GetByToken(string refreshToken);
        Task Update(RefreshToken refreshToken);
        Task<RefreshToken?> GetByTokenWithUser(string refreshToken);
        Task RevokeAllByUserId(Guid userId);
    }
}
