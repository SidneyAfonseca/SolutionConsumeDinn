using DIn.Domain;
using DIn.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ConsumeDinnApi.Controllers;

/// <summary>
/// Cadastro da empresa (matriz + filiais) e das relações De/Para de contas e
/// departamentos. Consulta é liberada para qualquer usuário autenticado, já que
/// a tela de envio depende desses dados; a manutenção é restrita ao Admin.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DealersController : ControllerBase
{
    private readonly AppDbContext _context;

    public DealersController(AppDbContext context)
    {
        _context = context;
    }

    // ----- empresa -----

    [HttpGet]
    public async Task<IActionResult> GetDealers()
    {
        var dealers = await _context.Dealers
            .Include(d => d.Branches)
            .OrderBy(d => d.CorporateName)
            .ToListAsync();

        var contas = await _context.AccountMappings
            .GroupBy(m => m.DealerId)
            .Select(g => new { DealerId = g.Key, Total = g.Count() })
            .ToListAsync();

        var departamentos = await _context.DepartmentMappings
            .GroupBy(m => m.DealerId)
            .Select(g => new { DealerId = g.Key, Total = g.Count() })
            .ToListAsync();

        return Ok(dealers.Select(d => Map(d,
            contas.FirstOrDefault(c => c.DealerId == d.Id)?.Total ?? 0,
            departamentos.FirstOrDefault(c => c.DealerId == d.Id)?.Total ?? 0)));
    }

    [HttpPost]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<IActionResult> CreateDealer([FromBody] SaveDealerRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var cnpj = request.HeadOfficeCnpj.Trim();

        if (await _context.Dealers.AnyAsync(d => d.HeadOfficeCnpj == cnpj))
            return Conflict(new { message = $"Já existe empresa cadastrada com o CNPJ matriz {cnpj}." });

        var erro = await ValidarDepartamentoPadrao(request.DefaultDepartmentCode);
        if (erro != null)
            return BadRequest(new { message = erro });

        var dealer = new Dealer
        {
            CorporateName = request.CorporateName.Trim(),
            HeadOfficeCnpj = cnpj,
            DefaultDepartmentCode = string.IsNullOrWhiteSpace(request.DefaultDepartmentCode)
                ? null
                : request.DefaultDepartmentCode.Trim(),
            IsActive = request.IsActive,
            CreatedAt = DateTime.UtcNow
        };

        _context.Dealers.Add(dealer);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetDealers), new { id = dealer.Id }, Map(dealer, 0, 0));
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<IActionResult> UpdateDealer(int id, [FromBody] SaveDealerRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var dealer = await _context.Dealers.Include(d => d.Branches).FirstOrDefaultAsync(d => d.Id == id);
        if (dealer == null)
            return NotFound(new { message = "Empresa não encontrada." });

        var cnpj = request.HeadOfficeCnpj.Trim();
        if (await _context.Dealers.AnyAsync(d => d.HeadOfficeCnpj == cnpj && d.Id != id))
            return Conflict(new { message = $"Já existe empresa cadastrada com o CNPJ matriz {cnpj}." });

        var erro = await ValidarDepartamentoPadrao(request.DefaultDepartmentCode);
        if (erro != null)
            return BadRequest(new { message = erro });

        dealer.CorporateName = request.CorporateName.Trim();
        dealer.HeadOfficeCnpj = cnpj;
        dealer.DefaultDepartmentCode = string.IsNullOrWhiteSpace(request.DefaultDepartmentCode)
            ? null
            : request.DefaultDepartmentCode.Trim();
        dealer.IsActive = request.IsActive;

        await _context.SaveChangesAsync();
        return Ok(new { message = "Empresa atualizada." });
    }

    // ----- filiais -----

    [HttpPost("{dealerId:int}/branches")]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<IActionResult> CreateBranch(int dealerId, [FromBody] SaveBranchRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (!await _context.Dealers.AnyAsync(d => d.Id == dealerId))
            return NotFound(new { message = "Empresa não encontrada." });

        var cnpj = request.Cnpj.Trim();

        if (await _context.DealerBranches.AnyAsync(b => b.DealerId == dealerId && b.Cnpj == cnpj))
            return Conflict(new { message = $"A filial {cnpj} já está cadastrada nesta empresa." });

        var branch = new DealerBranch
        {
            DealerId = dealerId,
            Cnpj = cnpj,
            Name = request.Name?.Trim(),
            IsHeadOffice = request.IsHeadOffice,
            IsActive = request.IsActive
        };

        _context.DealerBranches.Add(branch);
        await _context.SaveChangesAsync();

        return Ok(new DealerBranchDto
        {
            Id = branch.Id,
            Cnpj = branch.Cnpj,
            Name = branch.Name,
            IsHeadOffice = branch.IsHeadOffice,
            IsActive = branch.IsActive
        });
    }

    [HttpPut("branches/{branchId:int}/status")]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<IActionResult> UpdateBranchStatus(int branchId, [FromBody] UpdateUserStatusRequest request)
    {
        var branch = await _context.DealerBranches.FirstOrDefaultAsync(b => b.Id == branchId);
        if (branch == null)
            return NotFound(new { message = "Filial não encontrada." });

        branch.IsActive = request.IsActive;
        await _context.SaveChangesAsync();

        return Ok(new { message = branch.IsActive ? "Filial ativada." : "Filial inativada." });
    }

    [HttpDelete("branches/{branchId:int}")]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<IActionResult> DeleteBranch(int branchId)
    {
        var branch = await _context.DealerBranches.FirstOrDefaultAsync(b => b.Id == branchId);
        if (branch == null)
            return NotFound(new { message = "Filial não encontrada." });

        _context.DealerBranches.Remove(branch);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Filial removida." });
    }

    // ----- De/Para de contas -----

    [HttpGet("{dealerId:int}/account-mappings")]
    public async Task<IActionResult> GetAccountMappings(int dealerId)
    {
        var descricoes = await _context.AccountPlans
            .ToDictionaryAsync(a => a.Code, a => a.Description);

        var mappings = await _context.AccountMappings
            .Where(m => m.DealerId == dealerId)
            .OrderBy(m => m.SourceCode)
            .ToListAsync();

        return Ok(mappings.Select(m => new AccountMappingDto
        {
            Id = m.Id,
            SourceCode = m.SourceCode,
            SourceDescription = m.SourceDescription,
            AccountPlanCode = m.AccountPlanCode,
            AccountPlanDescription = descricoes.TryGetValue(m.AccountPlanCode, out var d) ? d : null,
            IsActive = m.IsActive
        }));
    }

    [HttpPost("{dealerId:int}/account-mappings")]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<IActionResult> SaveAccountMapping(int dealerId, [FromBody] SaveAccountMappingRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (!await _context.Dealers.AnyAsync(d => d.Id == dealerId))
            return NotFound(new { message = "Empresa não encontrada." });

        var destino = request.AccountPlanCode.Trim();
        if (!await _context.AccountPlans.AnyAsync(a => a.Code == destino))
            return BadRequest(new { message = $"O código {destino} não existe no plano de contas do D-IN." });

        var origem = request.SourceCode.Trim();
        var mapping = await _context.AccountMappings
            .FirstOrDefaultAsync(m => m.DealerId == dealerId && m.SourceCode == origem);

        if (mapping == null)
        {
            mapping = new AccountMapping { DealerId = dealerId, SourceCode = origem };
            _context.AccountMappings.Add(mapping);
        }

        mapping.SourceDescription = request.SourceDescription?.Trim();
        mapping.AccountPlanCode = destino;
        mapping.IsActive = request.IsActive;

        await _context.SaveChangesAsync();
        return Ok(new { message = $"De/Para da conta {origem} salvo." });
    }

    [HttpDelete("account-mappings/{id:int}")]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<IActionResult> DeleteAccountMapping(int id)
    {
        var mapping = await _context.AccountMappings.FirstOrDefaultAsync(m => m.Id == id);
        if (mapping == null)
            return NotFound(new { message = "De/Para não encontrado." });

        _context.AccountMappings.Remove(mapping);
        await _context.SaveChangesAsync();
        return Ok(new { message = "De/Para removido." });
    }

    // ----- De/Para de departamentos -----

    [HttpGet("{dealerId:int}/department-mappings")]
    public async Task<IActionResult> GetDepartmentMappings(int dealerId)
    {
        var descricoes = await _context.Departments
            .ToDictionaryAsync(d => d.Code, d => d.Description);

        var mappings = await _context.DepartmentMappings
            .Where(m => m.DealerId == dealerId)
            .OrderBy(m => m.SourceCode)
            .ToListAsync();

        return Ok(mappings.Select(m => new DepartmentMappingDto
        {
            Id = m.Id,
            SourceCode = m.SourceCode,
            SourceDescription = m.SourceDescription,
            DepartmentCode = m.DepartmentCode,
            DepartmentDescription = descricoes.TryGetValue(m.DepartmentCode, out var d) ? d : null,
            IsActive = m.IsActive
        }));
    }

    [HttpPost("{dealerId:int}/department-mappings")]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<IActionResult> SaveDepartmentMapping(int dealerId, [FromBody] SaveDepartmentMappingRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (!await _context.Dealers.AnyAsync(d => d.Id == dealerId))
            return NotFound(new { message = "Empresa não encontrada." });

        var destino = request.DepartmentCode.Trim();
        if (!await _context.Departments.AnyAsync(d => d.Code == destino))
            return BadRequest(new { message = $"O departamento {destino} não existe no D-IN." });

        var origem = request.SourceCode.Trim();
        var mapping = await _context.DepartmentMappings
            .FirstOrDefaultAsync(m => m.DealerId == dealerId && m.SourceCode == origem);

        if (mapping == null)
        {
            mapping = new DepartmentMapping { DealerId = dealerId, SourceCode = origem };
            _context.DepartmentMappings.Add(mapping);
        }

        mapping.SourceDescription = request.SourceDescription?.Trim();
        mapping.DepartmentCode = destino;
        mapping.IsActive = request.IsActive;

        await _context.SaveChangesAsync();
        return Ok(new { message = $"De/Para do centro de custo {origem} salvo." });
    }

    [HttpDelete("department-mappings/{id:int}")]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<IActionResult> DeleteDepartmentMapping(int id)
    {
        var mapping = await _context.DepartmentMappings.FirstOrDefaultAsync(m => m.Id == id);
        if (mapping == null)
            return NotFound(new { message = "De/Para não encontrado." });

        _context.DepartmentMappings.Remove(mapping);
        await _context.SaveChangesAsync();
        return Ok(new { message = "De/Para removido." });
    }

    private async Task<string?> ValidarDepartamentoPadrao(string? code)
    {
        if (string.IsNullOrWhiteSpace(code))
            return null;

        var existe = await _context.Departments.AnyAsync(d => d.Code == code.Trim());
        return existe ? null : $"O departamento padrão {code.Trim()} não existe no D-IN.";
    }

    private static DealerDto Map(Dealer dealer, int totalContas, int totalDepartamentos) => new()
    {
        Id = dealer.Id,
        CorporateName = dealer.CorporateName,
        HeadOfficeCnpj = dealer.HeadOfficeCnpj,
        DefaultDepartmentCode = dealer.DefaultDepartmentCode,
        IsActive = dealer.IsActive,
        CreatedAt = dealer.CreatedAt,
        TotalContasDePara = totalContas,
        TotalDepartamentosDePara = totalDepartamentos,
        Branches = dealer.Branches
            .OrderByDescending(b => b.IsHeadOffice)
            .ThenBy(b => b.Cnpj)
            .Select(b => new DealerBranchDto
            {
                Id = b.Id,
                Cnpj = b.Cnpj,
                Name = b.Name,
                IsHeadOffice = b.IsHeadOffice,
                IsActive = b.IsActive
            }).ToList()
    };
}
