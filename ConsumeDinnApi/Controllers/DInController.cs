using System.Security.Claims;
using DIn.Domain;
using DIn.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ConsumeDinnApi.Controllers;

[ApiController]
[Route("api/din")]
[Authorize]
public class DInController : ControllerBase
{
    private readonly IDInService _service;
    private readonly IBalanceteService _balanceteService;

    public DInController(IDInService service, IBalanceteService balanceteService)
    {
        _service = service;
        _balanceteService = balanceteService;
    }

    private int UsuarioLogadoId =>
        int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : 0;

    private string UsuarioLogadoNome => User.Identity?.Name ?? string.Empty;

    private bool EhAdmin => User.IsInRole(nameof(UserRole.Admin));

    [HttpPost]
    public async Task<IActionResult> Enviar([FromBody] List<DInRequest> request)
    {
        var result = await _service.EnviarAsync(request, UsuarioLogadoId, UsuarioLogadoNome);
        return Ok(result);
    }

    /// <summary>
    /// Aplica o De/Para no balancete e devolve o payload e os problemas
    /// encontrados, sem enviar nada para a AGCO.
    /// </summary>
    [HttpPost("balancete/validar")]
    public async Task<IActionResult> ValidarBalancete([FromBody] BalanceteRequest request)
    {
        var preview = await _balanceteService.ValidarAsync(request);
        return Ok(preview);
    }

    /// <summary>
    /// Valida e envia o balancete ao D-IN, registrando o envio com o usuário
    /// logado. O envio só ocorre se todas as validações do guia passarem.
    /// </summary>
    [HttpPost("balancete/enviar")]
    public async Task<IActionResult> EnviarBalancete([FromBody] BalanceteRequest request)
    {
        var resultado = await _balanceteService.EnviarAsync(request, UsuarioLogadoId, UsuarioLogadoNome);

        if (!resultado.Preview.Valido)
            return BadRequest(resultado);

        if (!resultado.Enviado)
            return StatusCode(StatusCodes.Status502BadGateway, resultado);

        return Ok(resultado);
    }

    /// <summary>
    /// Histórico de envios ao D-IN. Admin enxerga os envios de todos os
    /// usuários; um usuário normal enxerga apenas os próprios.
    /// </summary>
    [HttpGet("envios")]
    public async Task<IActionResult> Envios()
    {
        var envios = await _service.ListarEnviosAsync(EhAdmin ? null : UsuarioLogadoId);
        return Ok(envios);
    }
}
