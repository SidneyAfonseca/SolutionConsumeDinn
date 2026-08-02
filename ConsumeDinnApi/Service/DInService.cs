using DIn.Domain;
using DIn.Repository;

namespace DIn.Service;

public interface IDInService
{
    Task<DInResponse> EnviarAsync(List<DInRequest> data);
}

public class DInService : IDInService
{
    private readonly IDInRepository _repository;

    public DInService(IDInRepository repository)
    {
        _repository = repository;
    }

    public async Task<DInResponse> EnviarAsync(List<DInRequest> data)
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

        return await _repository.EnviarAsync(data);
    }
}
