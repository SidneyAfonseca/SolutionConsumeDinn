using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Components.Authorization;

namespace ConsumeDinnFront.Auth;

/// <summary>
/// Monta o estado de autenticação a partir das claims do JWT guardado no
/// localStorage. Um token expirado é descartado.
/// </summary>
public class JwtAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly TokenStore _store;
    private static readonly AuthenticationState Anonimo =
        new(new ClaimsPrincipal(new ClaimsIdentity()));

    public JwtAuthenticationStateProvider(TokenStore store)
    {
        _store = store;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = await _store.GetTokenAsync();
        if (string.IsNullOrWhiteSpace(token))
            return Anonimo;

        var claims = ParseClaims(token).ToList();
        if (claims.Count == 0 || Expirado(claims))
        {
            await _store.ClearAsync();
            return Anonimo;
        }

        var identity = new ClaimsIdentity(claims, "jwt", ClaimTypes.Name, ClaimTypes.Role);
        return new AuthenticationState(new ClaimsPrincipal(identity));
    }

    public async Task NotificarLoginAsync(string token)
    {
        await _store.SetTokenAsync(token);
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    public async Task NotificarLogoutAsync()
    {
        await _store.ClearAsync();
        NotifyAuthenticationStateChanged(Task.FromResult(Anonimo));
    }

    private static bool Expirado(IEnumerable<Claim> claims)
    {
        var exp = claims.FirstOrDefault(c => c.Type == "exp")?.Value;
        if (!long.TryParse(exp, out var segundos))
            return true;

        return DateTimeOffset.FromUnixTimeSeconds(segundos) <= DateTimeOffset.UtcNow;
    }

    /// <summary>
    /// Lê as claims do payload do JWT. O token não é validado aqui — quem
    /// valida assinatura e expiração de verdade é a API.
    /// </summary>
    private static IEnumerable<Claim> ParseClaims(string token)
    {
        var partes = token.Split('.');
        if (partes.Length < 2)
            return Enumerable.Empty<Claim>();

        Dictionary<string, JsonElement>? payload;
        try
        {
            payload = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(
                Convert.FromBase64String(Padronizar(partes[1])));
        }
        catch (Exception)
        {
            return Enumerable.Empty<Claim>();
        }

        if (payload == null)
            return Enumerable.Empty<Claim>();

        var claims = new List<Claim>();
        foreach (var (chave, valor) in payload)
        {
            var tipo = chave switch
            {
                "role" => ClaimTypes.Role,
                "unique_name" => ClaimTypes.Name,
                "nameid" => ClaimTypes.NameIdentifier,
                _ => chave
            };

            if (valor.ValueKind == JsonValueKind.Array)
            {
                foreach (var item in valor.EnumerateArray())
                    claims.Add(new Claim(tipo, item.ToString()));
            }
            else
            {
                claims.Add(new Claim(tipo, valor.ToString()));
            }
        }

        return claims;
    }

    private static string Padronizar(string base64Url)
    {
        var s = base64Url.Replace('-', '+').Replace('_', '/');
        return (s.Length % 4) switch
        {
            2 => s + "==",
            3 => s + "=",
            _ => s
        };
    }
}
