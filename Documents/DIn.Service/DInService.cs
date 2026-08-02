
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

        //var total = data.Sum(x => x.BalanceValue);
        //if (total != 0)
        //    throw new Exception("Total deve ser 0");

        return await _repository.EnviarAsync(data);
    }
}
