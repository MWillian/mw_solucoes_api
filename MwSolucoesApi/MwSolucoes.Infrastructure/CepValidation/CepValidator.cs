using MwSolucoes.Domain.CepValidation;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace MwSolucoes.Infrastructure.CepValidation
{
    public class CepValidator : ICepValidator
    {
        private readonly HttpClient _httpClient;

        public CepValidator(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<bool> IsValidCepAsync(string cep)
        {
            if (string.IsNullOrWhiteSpace(cep))
            {
                return false;
            }

            var clearCep = new string(cep.Where(char.IsDigit).ToArray());
            if (clearCep.Length != 8)
            {
                return false;
            }

            try
            {
                using HttpResponseMessage response = await _httpClient.GetAsync($"{clearCep}/json/");
                if (!response.IsSuccessStatusCode)
                {
                    return false;
                }

                var viaCepResponse = await response.Content.ReadFromJsonAsync<ViaCepResponse>();
                if (viaCepResponse is null)
                {
                    return false;
                }

                return viaCepResponse.Erro != true;
            }
            catch (HttpRequestException)
            {
                return false;
            }
            catch (TaskCanceledException)
            {
                return false;
            }
        }

        private sealed class ViaCepResponse
        {
            [JsonPropertyName("erro")]
            public bool? Erro { get; set; }
        }
    }
}
