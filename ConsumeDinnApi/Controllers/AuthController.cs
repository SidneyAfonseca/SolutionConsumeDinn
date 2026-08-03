using System.Security.Claims;
using DIn.Domain;
using DIn.Repository;
using DIn.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ConsumeDinnApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ITokenService _tokenService;

    public AuthController(AppDbContext context, ITokenService tokenService)
    {
        _context = context;
        _tokenService = tokenService;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Username == request.Username);

        if (user == null || !PasswordHasher.Verify(request.Password, user.PasswordHash))
            return Unauthorized(new { message = "Usuário ou senha inválidos." });

        if (!user.IsActive)
            return StatusCode(StatusCodes.Status403Forbidden,
                new { message = "Usuário desabilitado. Procure um administrador." });

        return Ok(_tokenService.GerarToken(user));
    }

    /// <summary>
    /// Dados do usuário autenticado, usados pelo front para revalidar o token.
    /// </summary>
    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> Me()
    {
        var id = int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId) ? userId : 0;
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);

        if (user == null || !user.IsActive)
            return Unauthorized(new { message = "Usuário inválido ou desabilitado." });

        return Ok(new UserDto
        {
            Id = user.Id,
            Username = user.Username,
            Role = user.Role,
            IsActive = user.IsActive,
            CreatedAt = user.CreatedAt
        });
    }
}
