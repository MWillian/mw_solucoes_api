using MwSolucoes.Domain.Entities;

namespace MwSolucoes.Domain.Security.Tokens
{
    public interface ITokenGenerator
    {
        string GenerateToken(User user);
    }
}
