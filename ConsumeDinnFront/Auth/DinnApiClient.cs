using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace ConsumeDinnFront.Auth;

/// <summary>
/// Erro vindo da API já com a mensagem pronta para exibição.
/// </summary>
public class ApiException : Exception
{
    public HttpStatusCode StatusCode { get; }

    public ApiException(HttpStatusCode statusCode, string message) : base(message)
    {
        StatusCode = statusCode;
    }
}

public class DinnApiClient
{
    private readonly HttpClient _http;
    private readonly JwtAuthenticationStateProvider _authProvider;

    public DinnApiClient(HttpClient http, JwtAuthenticationStateProvider authProvider)
    {
        _http = http;
        _authProvider = authProvider;
    }

    // ----- autenticação -----

    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        var response = await _http.PostAsJsonAsync("api/auth/login", request);
        await GarantirSucesso(response);

        var login = await response.Content.ReadFromJsonAsync<LoginResponse>()
            ?? throw new ApiException(HttpStatusCode.InternalServerError, "Resposta de login inválida.");

        await _authProvider.NotificarLoginAsync(login.Token);
        return login;
    }

    public Task LogoutAsync() => _authProvider.NotificarLogoutAsync();

    // ----- administração de usuários (endpoints exigem perfil Admin) -----

    public async Task<List<UserDto>> GetUsuariosAsync()
    {
        var response = await _http.GetAsync("api/users");
        await GarantirSucesso(response);
        return await response.Content.ReadFromJsonAsync<List<UserDto>>() ?? new List<UserDto>();
    }

    public async Task CriarUsuarioAsync(CreateUserRequest request)
    {
        var response = await _http.PostAsJsonAsync("api/users", request);
        await GarantirSucesso(response);
    }

    public async Task AlterarStatusAsync(int id, bool ativo)
    {
        var response = await _http.PutAsJsonAsync($"api/users/{id}/status",
            new UpdateUserStatusRequest { IsActive = ativo });
        await GarantirSucesso(response);
    }

    public async Task AlterarPerfilAsync(int id, UserRole role)
    {
        var response = await _http.PutAsJsonAsync($"api/users/{id}/role",
            new UpdateUserRoleRequest { Role = role });
        await GarantirSucesso(response);
    }

    public async Task RedefinirSenhaAsync(int id, string senha)
    {
        var response = await _http.PutAsJsonAsync($"api/users/{id}/password",
            new ResetPasswordRequest { Password = senha });
        await GarantirSucesso(response);
    }

    // ----- empresa e De/Para -----

    public async Task<List<DealerDto>> GetEmpresasAsync()
    {
        var response = await _http.GetAsync("api/dealers");
        await GarantirSucesso(response);
        return await response.Content.ReadFromJsonAsync<List<DealerDto>>() ?? new List<DealerDto>();
    }

    public async Task CriarEmpresaAsync(SaveDealerRequest request)
    {
        var response = await _http.PostAsJsonAsync("api/dealers", request);
        await GarantirSucesso(response);
    }

    public async Task AtualizarEmpresaAsync(int id, SaveDealerRequest request)
    {
        var response = await _http.PutAsJsonAsync($"api/dealers/{id}", request);
        await GarantirSucesso(response);
    }

    public async Task CriarFilialAsync(int dealerId, SaveBranchRequest request)
    {
        var response = await _http.PostAsJsonAsync($"api/dealers/{dealerId}/branches", request);
        await GarantirSucesso(response);
    }

    public async Task AlterarStatusFilialAsync(int branchId, bool ativa)
    {
        var response = await _http.PutAsJsonAsync($"api/dealers/branches/{branchId}/status",
            new UpdateUserStatusRequest { IsActive = ativa });
        await GarantirSucesso(response);
    }

    public async Task RemoverFilialAsync(int branchId)
    {
        var response = await _http.DeleteAsync($"api/dealers/branches/{branchId}");
        await GarantirSucesso(response);
    }

    public async Task<List<AccountMappingDto>> GetDeParaContasAsync(int dealerId)
    {
        var response = await _http.GetAsync($"api/dealers/{dealerId}/account-mappings");
        await GarantirSucesso(response);
        return await response.Content.ReadFromJsonAsync<List<AccountMappingDto>>() ?? new List<AccountMappingDto>();
    }

    public async Task SalvarDeParaContaAsync(int dealerId, SaveAccountMappingRequest request)
    {
        var response = await _http.PostAsJsonAsync($"api/dealers/{dealerId}/account-mappings", request);
        await GarantirSucesso(response);
    }

    public async Task RemoverDeParaContaAsync(int id)
    {
        var response = await _http.DeleteAsync($"api/dealers/account-mappings/{id}");
        await GarantirSucesso(response);
    }

    public async Task<List<DepartmentMappingDto>> GetDeParaDepartamentosAsync(int dealerId)
    {
        var response = await _http.GetAsync($"api/dealers/{dealerId}/department-mappings");
        await GarantirSucesso(response);
        return await response.Content.ReadFromJsonAsync<List<DepartmentMappingDto>>() ?? new List<DepartmentMappingDto>();
    }

    public async Task SalvarDeParaDepartamentoAsync(int dealerId, SaveDepartmentMappingRequest request)
    {
        var response = await _http.PostAsJsonAsync($"api/dealers/{dealerId}/department-mappings", request);
        await GarantirSucesso(response);
    }

    public async Task RemoverDeParaDepartamentoAsync(int id)
    {
        var response = await _http.DeleteAsync($"api/dealers/department-mappings/{id}");
        await GarantirSucesso(response);
    }

    public async Task<List<CodeDescriptionDto>> GetDepartamentosAsync()
    {
        var response = await _http.GetAsync("api/config/departments");
        await GarantirSucesso(response);
        return await response.Content.ReadFromJsonAsync<List<CodeDescriptionDto>>() ?? new List<CodeDescriptionDto>();
    }

    public async Task<List<CodeDescriptionDto>> GetPlanoDeContasAsync()
    {
        var response = await _http.GetAsync("api/config/accountplans");
        await GarantirSucesso(response);
        return await response.Content.ReadFromJsonAsync<List<CodeDescriptionDto>>() ?? new List<CodeDescriptionDto>();
    }

    // ----- balancete -----

    public async Task<BalancetePreview> ValidarBalanceteAsync(BalanceteRequest request)
    {
        var response = await _http.PostAsJsonAsync("api/din/balancete/validar", request);
        await GarantirSucesso(response);
        return await response.Content.ReadFromJsonAsync<BalancetePreview>() ?? new BalancetePreview();
    }

    /// <summary>
    /// Envia o balancete. Um 400 (validação) ou 502 (recusa da AGCO) devolve o
    /// resultado com os erros em vez de lançar exceção, para a tela mostrá-los.
    /// </summary>
    public async Task<BalanceteEnvioResultado> EnviarBalanceteAsync(BalanceteRequest request)
    {
        var response = await _http.PostAsJsonAsync("api/din/balancete/enviar", request);

        if (response.StatusCode is HttpStatusCode.BadRequest or HttpStatusCode.BadGateway)
        {
            var comErro = await response.Content.ReadFromJsonAsync<BalanceteEnvioResultado>();
            if (comErro != null)
                return comErro;
        }

        await GarantirSucesso(response);
        return await response.Content.ReadFromJsonAsync<BalanceteEnvioResultado>() ?? new BalanceteEnvioResultado();
    }

    // ----- envios ao D-IN -----

    public async Task<List<DInReturnDto>> GetEnviosAsync()
    {
        var response = await _http.GetAsync("api/din/envios");
        await GarantirSucesso(response);
        return await response.Content.ReadFromJsonAsync<List<DInReturnDto>>() ?? new List<DInReturnDto>();
    }

    /// <summary>
    /// Traduz o status HTTP em uma exceção com a mensagem da API. Um 401
    /// derruba a sessão local, pois o token não vale mais.
    /// </summary>
    private async Task GarantirSucesso(HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode)
            return;

        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            await _authProvider.NotificarLogoutAsync();
            throw new ApiException(response.StatusCode, "Sessão expirada ou credenciais inválidas.");
        }

        var mensagem = await LerMensagem(response);
        throw new ApiException(response.StatusCode, mensagem);
    }

    private static async Task<string> LerMensagem(HttpResponseMessage response)
    {
        var texto = await response.Content.ReadAsStringAsync();

        if (string.IsNullOrWhiteSpace(texto))
            return $"Erro {(int)response.StatusCode} ao chamar a API.";

        try
        {
            var erro = JsonSerializer.Deserialize<ApiError>(texto,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (!string.IsNullOrWhiteSpace(erro?.Message))
                return erro.Message;
        }
        catch (JsonException)
        {
            // corpo não é JSON — devolve o texto puro
        }

        return texto;
    }
}
