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
        private async Task SendRawEmailAsync(string to, string subject, string htmlBody)
        {
            var message = new EmailMessage
            {
                From = $"MW Soluções <{_fromEmail}>",
                To = to,
                Subject = subject,
                HtmlBody = htmlBody
            };

            await _resend.EmailSendAsync(message);
        }
    }
}
