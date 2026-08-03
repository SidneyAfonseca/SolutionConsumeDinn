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

    public DInController(IDInService service)
    {
        _service = service;
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
