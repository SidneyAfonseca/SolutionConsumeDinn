using System.Security.Claims;
using DIn.Domain;
using DIn.Repository;
using DIn.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ConsumeDinnApi.Controllers;

/// <summary>
/// Administração de usuários. Todo o controller exige o perfil Admin:
/// somente um administrador cria usuários, troca perfis e desabilita acessos.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = nameof(UserRole.Admin))]
public class UsersController : ControllerBase
{
    private readonly AppDbContext _context;

    public UsersController(AppDbContext context)
    {
        _context = context;
    }

    private int UsuarioLogadoId =>
        int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : 0;

    [HttpGet]
    public async Task<IActionResult> GetUsers()
    {
        var envios = await _context.DInReturns
            .Where(r => r.UserId != null)
            .GroupBy(r => r.UserId!.Value)
            .Select(g => new { UserId = g.Key, Total = g.Count(), Ultimo = g.Max(r => r.CreatedAt) })
            .ToListAsync();

        var users = await _context.Users
            .OrderBy(u => u.Username)
            .ToListAsync();

        var result = users.Select(u =>
        {
            var e = envios.FirstOrDefault(x => x.UserId == u.Id);
            return new UserDto
            {
                Id = u.Id,
                Username = u.Username,
                Role = u.Role,
                IsActive = u.IsActive,
                CreatedAt = u.CreatedAt,
                TotalEnvios = e?.Total ?? 0,
                UltimoEnvio = e?.Ultimo
            };
        }).ToList();

        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var username = request.Username.Trim();

        if (await _context.Users.AnyAsync(u => u.Username == username))
            return Conflict(new { message = $"Já existe um usuário com o login '{username}'." });

        var user = new UserLogin
        {
            Username = username,
            PasswordHash = PasswordHasher.Hash(request.Password),
            Role = request.Role,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetUsers), new { id = user.Id }, new UserDto
        {
            Id = user.Id,
            Username = user.Username,
            Role = user.Role,
            IsActive = user.IsActive,
            CreatedAt = user.CreatedAt
        });
    }

    /// <summary>
    /// Habilita ou desabilita um usuário. Um admin não pode desabilitar a si
    /// mesmo, nem remover o último admin ativo do sistema.
    /// </summary>
    [HttpPut("{id:int}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateUserStatusRequest request)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
        if (user == null)
            return NotFound(new { message = "Usuário não encontrado." });

        if (id == UsuarioLogadoId)
            return BadRequest(new { message = "Você não pode desabilitar o seu próprio usuário." });

        if (!request.IsActive && await UltimoAdminAtivo(user))
            return BadRequest(new { message = "É necessário manter ao menos um administrador ativo." });

        user.IsActive = request.IsActive;
        await _context.SaveChangesAsync();

        return Ok(new { message = user.IsActive ? "Usuário habilitado." : "Usuário desabilitado." });
    }

    /// <summary>
    /// Troca o perfil do usuário entre Admin e UserNormal.
    /// </summary>
    [HttpPut("{id:int}/role")]
    public async Task<IActionResult> UpdateRole(int id, [FromBody] UpdateUserRoleRequest request)
    {
        if (!Enum.IsDefined(typeof(UserRole), request.Role))
            return BadRequest(new { message = "Perfil inválido." });

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
        if (user == null)
            return NotFound(new { message = "Usuário não encontrado." });

        if (id == UsuarioLogadoId && request.Role != UserRole.Admin)
            return BadRequest(new { message = "Você não pode remover o seu próprio perfil de administrador." });

        if (request.Role != UserRole.Admin && await UltimoAdminAtivo(user))
            return BadRequest(new { message = "É necessário manter ao menos um administrador ativo." });

        user.Role = request.Role;
        await _context.SaveChangesAsync();

        return Ok(new { message = $"Perfil alterado para {user.Role}." });
    }

    [HttpPut("{id:int}/password")]
    public async Task<IActionResult> ResetPassword(int id, [FromBody] ResetPasswordRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
        if (user == null)
            return NotFound(new { message = "Usuário não encontrado." });

        user.PasswordHash = PasswordHasher.Hash(request.Password);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Senha redefinida." });
    }

    private async Task<bool> UltimoAdminAtivo(UserLogin user)
    {
        if (user.Role != UserRole.Admin || !user.IsActive)
            return false;

        var adminsAtivos = await _context.Users
            .CountAsync(u => u.Role == UserRole.Admin && u.IsActive);

        return adminsAtivos <= 1;
    }
}
