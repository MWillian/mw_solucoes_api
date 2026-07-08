using MwSolucoes.Domain.TemplateRenderer;
using Microsoft.AspNetCore.Hosting;

namespace MwSolucoes.Infrastructure.Communication
{
    public class TemplateRenderer : ITemplateRenderer
    {
        private readonly string _templateBasePath;
        public TemplateRenderer()
        {
            string baseDirectory = AppContext.BaseDirectory;
            _templateBasePath = Path.GetFullPath(Path.Combine(baseDirectory, "..", "..", "..", "..", "MwSolucoes.Infrastructure", "Templates", "Email"));
        }
        public async Task<string> RenderAsync(string templateName, Dictionary<string, string> replacements)
        {
            if (!templateName.EndsWith(".html"))
            {
                templateName += ".html";
            }
            string filePath = Path.Combine(_templateBasePath, templateName);
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"O Template de email '{templateName}' não foi encontrado no caminho'{_templateBasePath}'.");
            }
            string htmlContent = await File.ReadAllTextAsync(filePath);
            foreach (var replacement in replacements)
            {
                htmlContent = htmlContent.Replace($"{{{{{replacement.Key}}}}}", replacement.Value);
            }
            return htmlContent;
        }
    }
}