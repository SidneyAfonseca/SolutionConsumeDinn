using Microsoft.JSInterop;

namespace ConsumeDinnFront.Auth;

/// <summary>
/// Guarda o token JWT no localStorage do navegador.
/// </summary>
public class TokenStore
{
    private const string Key = "dinn.token";
    private readonly IJSRuntime _js;
    private string? _cache;

    public TokenStore(IJSRuntime js)
    {
        _js = js;
    }

    public async Task<string?> GetTokenAsync()
    {
        if (!string.IsNullOrEmpty(_cache))
            return _cache;

        _cache = await _js.InvokeAsync<string?>("localStorage.getItem", Key);
        return _cache;
    }

    public async Task SetTokenAsync(string token)
    {
        _cache = token;
        await _js.InvokeVoidAsync("localStorage.setItem", Key, token);
    }

    public async Task ClearAsync()
    {
        _cache = null;
        await _js.InvokeVoidAsync("localStorage.removeItem", Key);
    }
}
