namespace MwSolucoes.Domain.TemplateRenderer
{
    public interface ITemplateRenderer
    {
        Task<string> RenderAsync(string templateName, Dictionary<string, string> replacements);
    }
}
