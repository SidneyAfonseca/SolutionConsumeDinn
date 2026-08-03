using System.Net.Http.Headers;

namespace ConsumeDinnFront.Auth;

/// <summary>
/// Anexa o token JWT em toda requisição para a API.
/// </summary>
public class BearerTokenHandler : DelegatingHandler
{
    private readonly TokenStore _store;

    public BearerTokenHandler(TokenStore store)
    {
        _store = store;
        InnerHandler = new HttpClientHandler();
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = await _store.GetTokenAsync();

        if (!string.IsNullOrWhiteSpace(token))
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        return await base.SendAsync(request, cancellationToken);
    }
}
