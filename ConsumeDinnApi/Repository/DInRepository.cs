using System.Net.Http.Json;
using System.Text.Json;
using DIn.Domain;
using Microsoft.Extensions.Configuration;

namespace DIn.Repository;

public interface IDInRepository
{
    Task<DInResponse> EnviarAsync(List<DInRequest> data);
}

public class DInRepository : IDInRepository
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _config;

    public DInRepository(HttpClient httpClient, IConfiguration config)
    {
        _httpClient = httpClient;
        _config = config;
    }
    /// <summary>
    /// url de produçao: https://din-api.vwfs.com.br/api/v1/din
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public async Task<DInResponse> EnviarAsync(List<DInRequest> data)
    {
       var ambiente = _config["DIn:Ambiente"];
        var isQa = string.Equals(ambiente, "QA", StringComparison.OrdinalIgnoreCase)
            || string.Equals(ambiente, "Homolog", StringComparison.OrdinalIgnoreCase)
            || string.Equals(ambiente, "HML", StringComparison.OrdinalIgnoreCase);

        var url = isQa ? _config["DIn:UrlQa"] : _config["DIn:UrlPrd"];

        if (string.IsNullOrWhiteSpace(url))
            throw new Exception("Configuração de URL do DIn não encontrada.");

        var response = await _httpClient.PostAsJsonAsync(url, data);
        var content = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
            throw new Exception($"Erro API: {response.StatusCode} - {content}");

        return JsonSerializer.Deserialize<DInResponse>(content,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
    }

}
