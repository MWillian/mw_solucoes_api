namespace MwSolucoes.Domain.Communication
{
    public interface IEmailService
    {
        Task SendWelcomeConfirmationAsync(string toEmail, string customerName, string confirmationLink);
        Task SendPasswordResetAsync(string toEmail, string customerName, string confirmationLink);
        Task SendOrderServiceProposalAsync(string toEmail, string customerName, string protocol, string totalValue, byte[] pdfAttachment, string fileName);
        Task SendOrderServiceReceiptAsync(string toEmail, string customerName, string protocol, string totalValue, byte[] pdfAttachment, string fileName);
    }
}
