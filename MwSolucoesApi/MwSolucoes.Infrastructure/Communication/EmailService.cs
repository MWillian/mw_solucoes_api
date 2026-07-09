using Microsoft.Extensions.Configuration;
using MwSolucoes.Domain.Communication;
using MwSolucoes.Domain.TemplateRenderer;
using Resend;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace MwSolucoes.Infrastructure.Communication
{
    public class EmailService : IEmailService
    {
        private readonly IResend _resend;
        private readonly string _fromEmail;
        private readonly ITemplateRenderer _templateRenderer;
        public EmailService(IConfiguration configuration, ITemplateRenderer templateRenderer)
        {
            var apiKey = configuration["Resend:ApiKey"] ?? throw new ArgumentNullException("Resend:ApiKey não configurada.");
            _fromEmail = configuration["Resend:FromEmail"] ?? "onboarding@resend.dev";
            _resend = ResendClient.Create(apiKey);
            _templateRenderer = templateRenderer;
        }

        public async Task SendOrderServiceProposalAsync(string toEmail, string customerName, string protocol, string totalValue, byte[] pdfAttachment, string fileName)
        {
            var replacements = new Dictionary<string, string>
            {
                { "CustomerName", customerName },
                { "Protocol", protocol },
                { "TotalValue", totalValue }
            };

            string htmlBody = await _templateRenderer.RenderAsync("order-service-prop", replacements);

            await SendRawEmailAsync(
                toEmail,
                $"Nova Ordem de Serviço Disponível (OS: #{protocol}) - MW Soluções",
                htmlBody,
                pdfAttachment,
                fileName
            );
        }

        public async Task SendOrderServiceReceiptAsync(string toEmail, string customerName, string protocol, string totalValue, byte[] pdfAttachment, string fileName)
        {
            var replacements = new Dictionary<string, string>
            {
                { "CustomerName", customerName },
                { "Protocol", protocol },
                { "TotalValue", totalValue }
            };

            string htmlBody = await _templateRenderer.RenderAsync("order-service-receipt", replacements);

            await SendRawEmailAsync(
                toEmail,
                $"Seu Recibo de Quitação (OS: #{protocol}) - MW Soluções",
                htmlBody,
                pdfAttachment,
                fileName
            );
        }

        public async Task SendPasswordResetAsync(string toEmail, string customerName, string confirmationLink)
        {
            var replacements = new Dictionary<string, string>
            {
                { "CustomerName", customerName },
                { "ResetLink", confirmationLink },
            };

            string htmlBody = await _templateRenderer.RenderAsync("reset-password", replacements);

            await SendRawEmailAsync(toEmail, "Recuperação de senha - MW Soluções", htmlBody);
        }

        public async Task SendWelcomeConfirmationAsync(string toEmail, string customerName, string confirmationLink)
        {
            var replacements = new Dictionary<string, string>
            {
                { "CustomerName", customerName },
                { "ConfirmationLink", confirmationLink }
            };

            string htmlBody = await _templateRenderer.RenderAsync("confirm-account", replacements);

            await SendRawEmailAsync(toEmail, "Confirme sua conta - MW Soluções", htmlBody);
        }
        private async Task SendRawEmailAsync(string to, string subject, string htmlBody, byte[]? pdfAttachment = null, string? fileName = null)
        {
            var message = new EmailMessage
            {
                From = $"MW Soluções <{_fromEmail}>",
                To = to,
                Subject = subject,
                HtmlBody = htmlBody
            };
            if (pdfAttachment is not null && fileName is not null)
            {
                message.Attachments = new List<EmailAttachment>
                {
                    new EmailAttachment
                    {
                        Filename = fileName,
                        Content = Convert.ToBase64String(pdfAttachment)
                    }
                };
            }
            await _resend.EmailSendAsync(message);
        }
    }
}
