using System.Text.Json;
using DIn.Domain;
using DIn.Repository;
using Microsoft.EntityFrameworkCore;

namespace DIn.Service;

public interface IDInService
{
    Task<DInResponse> EnviarAsync(List<DInRequest> data, int userId, string username);
    /// <summary>
    /// Envios registrados. Quando <paramref name="userId"/> é informado,
    /// retorna apenas os envios daquele usuário.
    /// </summary>
    Task<List<DInReturnDto>> ListarEnviosAsync(int? userId);
}

public class DInService : IDInService
{
    private readonly IDInRepository _repository;
    private readonly AppDbContext _context;

    public DInService(IDInRepository repository, AppDbContext context)
    {
        _repository = repository;
        _context = context;
    }

    public async Task<DInResponse> EnviarAsync(List<DInRequest> data, int userId, string username)
    {
        if (data == null || !data.Any())
            throw new Exception("Payload vazio");

        var total = data.Sum(x => x.BalanceValue);
        if (total != 0)
            throw new Exception("A soma das contas de Ativo, Passivo e DRE devem ser igual a zero.");

        foreach (var item in data)
        {
            if (string.IsNullOrWhiteSpace(item.DealerHeadOfficeCode)) throw new Exception("O campo dealerHeadOfficeCode não pode ser nulo");
            if (item.DfsPk == null) throw new Exception("O campo dfsPk não pode ser nulo");
            if (string.IsNullOrWhiteSpace(item.DfsPk.CodeDFSPlan)) throw new Exception("O campo codeDFSPlan não pode ser nulo");
            if (string.IsNullOrWhiteSpace(item.DfsPk.DealerBranchCode)) throw new Exception("O campo dealerBranchCode não pode ser nulo");
            if (string.IsNullOrWhiteSpace(item.DfsPk.PeriodMonth)) throw new Exception("O campo periodMonth não pode ser nulo");
            if (!int.TryParse(item.DfsPk.PeriodMonth, out int month) || month < 1 || month > 12) throw new Exception("O campo periodMonth precisa estar entre 1 e 12");
            if (string.IsNullOrWhiteSpace(item.DfsPk.PeriodYear)) throw new Exception("O campo periodYear não pode ser nulo");
            if (item.DfsPk.PeriodYear.Length != 4) throw new Exception("O campo periodYear precisa ter 4 dígitos");
            if (string.IsNullOrWhiteSpace(item.DfsPk.SectionCode)) throw new Exception("O campo sectionCode não pode ser nulo");
        }

        var payload = JsonSerializer.Serialize(data);

        try
        {
            var response = await _repository.EnviarAsync(data);
            await RegistrarEnvioAsync(payload, JsonSerializer.Serialize(response), true, userId, username);
            return response;
        }
        catch (Exception ex)
        {
            await RegistrarEnvioAsync(payload, ex.Message, false, userId, username);
            throw;
        }
    }

    public async Task<List<DInReturnDto>> ListarEnviosAsync(int? userId)
    {
        var query = _context.DInReturns.AsQueryable();

        if (userId.HasValue)
            query = query.Where(r => r.UserId == userId.Value);

        return await query
            .OrderByDescending(r => r.CreatedAt)
            .Select(r => new DInReturnDto
            {
                Id = r.Id,
                CreatedAt = r.CreatedAt,
                UserId = r.UserId,
                Username = r.Username,
                Success = r.Success,
                Response = r.Response,
                Payload = r.Payload
            })
            .ToListAsync();
    }

    /// <summary>
    /// Grava o envio (com sucesso ou falha) junto do usuário que o disparou,
    /// para que a origem de cada envio ao D-IN seja rastreável.
    /// </summary>
    private async Task RegistrarEnvioAsync(string payload, string response, bool success, int userId, string username)
    {
        _context.DInReturns.Add(new DInReturn
        {
            CreatedAt = DateTime.UtcNow,
            Payload = payload,
            Response = response,
            Success = success,
            UserId = userId > 0 ? userId : null,
            Username = username
        });

        await _context.SaveChangesAsync();
    }
}
