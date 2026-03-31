namespace MwSolucoes.Domain.Security
{
    public interface IPasswordEncrypter
    {
        string Encrypt(string password);
        bool Verify(string password, string passwordHash);
    }
}
