using Microsoft.EntityFrameworkCore;
using MwSolucoes.Domain.Entities;
using MwSolucoes.Domain.Repositories;
using MwSolucoes.Infrastructure.Data;

namespace MwSolucoes.Infrastructure.Repositories
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly AppDbContext _context;
        public RefreshTokenRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task Add(RefreshToken refreshToken)
        {
            await _context.RefreshTokens.AddAsync(refreshToken);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(string refreshToken)
        {
            var _ = await _context.RefreshTokens.Where(rt => rt.Token == refreshToken).ExecuteDeleteAsync();
        }

        public async Task<RefreshToken?> GetByToken(string refreshToken)
        {
            return await _context.RefreshTokens.Where(rt => rt.Token == refreshToken).FirstOrDefaultAsync();
        }

        public async Task Update(RefreshToken refreshToken)
        {
            await _context.RefreshTokens.Where(rt => rt.Id == refreshToken.Id).ExecuteUpdateAsync(setters =>
                setters.SetProperty(rt => rt.IsRevoked, refreshToken.IsRevoked)
            );
        }
        public async Task<RefreshToken?> GetByTokenWithUser(string refreshToken)
        {
            return await _context.RefreshTokens.Include(rt => rt.User).Where(rt => rt.Token == refreshToken).FirstOrDefaultAsync();
        }

        public async Task RevokeAllByUserId(Guid userId)
        {
            var now = DateTime.UtcNow;
            var _ = await _context.RefreshTokens.Where(rt => rt.UserId == userId && !rt.IsRevoked && rt.ExpiresAt > now)
                .ExecuteUpdateAsync(setters =>
                    setters.SetProperty(rt => rt.IsRevoked, true)
                );
        }
    }
}
