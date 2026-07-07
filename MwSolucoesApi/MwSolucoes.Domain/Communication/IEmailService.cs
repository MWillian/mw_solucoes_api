namespace MwSolucoes.Domain.Communication
{
    public interface IEmailService
    {
        Task SendWelcomeConfirmationAsync(string toEmail, string customerName, string confirmationLink);
    }
}
