using MwSolucoes.Domain.Security.Cryptography;

namespace MwSolucoes.Infrastructure.Security.Cryptography
{
    public class Bcrypter : IPasswordEncrypter
    {
        public string Encrypt(string password)
        {
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(password);
            return passwordHash;
        }

        public bool Verify(string password, string passwordHash) => BCrypt.Net.BCrypt.Verify(password, passwordHash);
    }
}
