namespace MwSolucoes.Domain.CepValidation
{
    public interface ICepValidator
    {
        Task<bool> IsValidCepAsync(string cep);
    }
}
