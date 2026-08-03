using System.Text.Json;
using DIn.Domain;
using DIn.Repository;
using Microsoft.EntityFrameworkCore;

namespace DIn.Service;

public interface IBalanceteService
{
    /// <summary>
    /// Aplica o De/Para e valida o balancete conforme o guia, sem enviar nada.
    /// </summary>
    Task<BalancetePreview> ValidarAsync(BalanceteRequest request);

    /// <summary>
    /// Valida e, se estiver tudo certo, envia para a API do D-IN.
    /// </summary>
    Task<BalanceteEnvioResultado> EnviarAsync(BalanceteRequest request, int userId, string username);
}

public class BalanceteService : IBalanceteService
{
    /// <summary>Limite de corpo da requisição imposto pela AGCO (regra 16 do guia).</summary>
    private const long TamanhoMaximoBytes = 4 * 1024 * 1024;

    private readonly AppDbContext _context;
    private readonly IDInService _dInService;

    public BalanceteService(AppDbContext context, IDInService dInService)
    {
        _context = context;
        _dInService = dInService;
    }

    public async Task<BalancetePreview> ValidarAsync(BalanceteRequest request)
    {
        var preview = new BalancetePreview();

        var dealer = await _context.Dealers
            .Include(d => d.Branches)
            .FirstOrDefaultAsync(d => d.Id == request.DealerId);

        if (dealer == null)
        {
            preview.Erros.Add("Empresa não encontrada. Cadastre a empresa e as filiais antes de enviar.");
            return preview;
        }

        ValidarPeriodo(request, preview);

        var linhas = (request.Linhas ?? new List<BalanceteLinha>())
            .Where(l => !string.IsNullOrWhiteSpace(l.SourceAccountCode))
            .ToList();

        preview.TotalLinhasOrigem = linhas.Count;

        if (linhas.Count == 0)
        {
            preview.Erros.Add("Balancete vazio: informe ao menos uma linha com conta e saldo.");
            return preview;
        }

        var filiaisAtivas = dealer.Branches.Where(b => b.IsActive).ToList();
        if (filiaisAtivas.Count == 0)
        {
            // Operação com uma única unidade: HeadOffice = BranchCode (item 3.1 do guia).
            filiaisAtivas.Add(new DealerBranch
            {
                Cnpj = dealer.HeadOfficeCnpj,
                Name = dealer.CorporateName,
                IsHeadOffice = true,
                IsActive = true
            });
            preview.Avisos.Add(
                "Nenhuma filial cadastrada: os dados serão enviados apenas para a matriz " +
                $"({dealer.HeadOfficeCnpj}), com HeadOffice = BranchCode.");
        }

        var contasDePara = await _context.AccountMappings
            .Where(m => m.DealerId == dealer.Id && m.IsActive)
            .ToDictionaryAsync(m => Normalizar(m.SourceCode), m => m.AccountPlanCode);

        var departamentosDePara = await _context.DepartmentMappings
            .Where(m => m.DealerId == dealer.Id && m.IsActive)
            .ToDictionaryAsync(m => Normalizar(m.SourceCode), m => m.DepartmentCode);

        var planoDeContas = await _context.AccountPlans.Select(a => a.Code).ToListAsync();
        var departamentos = await _context.Departments.Select(d => d.Code).ToListAsync();
        var cnpjsAtivos = filiaisAtivas.Select(b => Normalizar(b.Cnpj)).ToHashSet();

        // Agrupa por PK (codeDFSPlan + filial + período + departamento): várias
        // contas de origem podem cair na mesma conta D-IN, e a mesma PK não pode
        // ser enviada duas vezes no mesmo envio (regra 13 do guia).
        var agrupado = new Dictionary<string, DInRequest>();
        var filiaisComLinha = new HashSet<string>();
        var contasSemDePara = new SortedSet<string>();
        var departamentosSemDePara = new SortedSet<string>();
        var codigosInvalidos = new SortedSet<string>();
        var departamentosInvalidos = new SortedSet<string>();
        var cnpjsDesconhecidos = new SortedSet<string>();

        foreach (var linha in linhas)
        {
            // Filial: em branco assume a matriz.
            var cnpj = string.IsNullOrWhiteSpace(linha.BranchCnpj)
                ? dealer.HeadOfficeCnpj
                : linha.BranchCnpj.Trim();

            if (!cnpjsAtivos.Contains(Normalizar(cnpj)))
            {
                cnpjsDesconhecidos.Add($"{cnpj} (linha {linha.Numero})");
                continue;
            }

            filiaisComLinha.Add(Normalizar(cnpj));

            // De/Para da conta — sem relação, o envio é bloqueado.
            var chaveConta = Normalizar(linha.SourceAccountCode);
            if (!contasDePara.TryGetValue(chaveConta, out var codeDfsPlan))
            {
                contasSemDePara.Add($"{linha.SourceAccountCode.Trim()} (linha {linha.Numero})");
                continue;
            }

            if (!planoDeContas.Contains(codeDfsPlan))
            {
                codigosInvalidos.Add($"{codeDfsPlan} (De/Para da conta {linha.SourceAccountCode.Trim()})");
                continue;
            }

            // De/Para do departamento — em branco usa o padrão da empresa.
            string? sectionCode;
            if (string.IsNullOrWhiteSpace(linha.SourceCostCenter))
            {
                sectionCode = dealer.DefaultDepartmentCode;
                if (string.IsNullOrWhiteSpace(sectionCode))
                {
                    departamentosSemDePara.Add(
                        $"(sem centro de custo, linha {linha.Numero}) — cadastre o departamento padrão da empresa");
                    continue;
                }
            }
            else if (!departamentosDePara.TryGetValue(Normalizar(linha.SourceCostCenter), out sectionCode))
            {
                departamentosSemDePara.Add($"{linha.SourceCostCenter.Trim()} (linha {linha.Numero})");
                continue;
            }

            if (!departamentos.Contains(sectionCode))
            {
                departamentosInvalidos.Add($"{sectionCode} (linha {linha.Numero})");
                continue;
            }

            var chave = $"{codeDfsPlan}|{cnpj}|{request.PeriodYear}|{request.PeriodMonth}|{sectionCode}";

            if (agrupado.TryGetValue(chave, out var item))
            {
                item.BalanceValue += linha.BalanceValue;
            }
            else
            {
                agrupado[chave] = new DInRequest
                {
                    BalanceValue = linha.BalanceValue,
                    DealerHeadOfficeCode = dealer.HeadOfficeCnpj,
                    DfsPk = new DfsPk
                    {
                        CodeDFSPlan = codeDfsPlan,
                        DealerBranchCode = cnpj,
                        PeriodMonth = request.PeriodMonth,
                        PeriodYear = request.PeriodYear,
                        SectionCode = sectionCode
                    }
                };
            }
        }

        preview.ContasSemDePara = contasSemDePara.ToList();
        preview.DepartamentosSemDePara = departamentosSemDePara.ToList();

        if (contasSemDePara.Count > 0)
            preview.Erros.Add(
                "Existem contas sem relação De/Para. Cadastre o De/Para dessas contas antes de enviar: " +
                string.Join("; ", contasSemDePara.Take(20)) +
                (contasSemDePara.Count > 20 ? $" e outras {contasSemDePara.Count - 20}." : "."));

        if (departamentosSemDePara.Count > 0)
            preview.Erros.Add(
                "Existem centros de custo sem relação De/Para: " +
                string.Join("; ", departamentosSemDePara.Take(20)) +
                (departamentosSemDePara.Count > 20 ? $" e outros {departamentosSemDePara.Count - 20}." : "."));

        if (codigosInvalidos.Count > 0)
            preview.Erros.Add("De/Para aponta para códigos que não existem no plano de contas D-IN: " +
                string.Join("; ", codigosInvalidos) + ".");

        if (departamentosInvalidos.Count > 0)
            preview.Erros.Add("De/Para aponta para departamentos que não existem no D-IN: " +
                string.Join("; ", departamentosInvalidos) + ".");

        if (cnpjsDesconhecidos.Count > 0)
            preview.Erros.Add("O balancete traz CNPJ que não é uma filial ativa da empresa: " +
                string.Join("; ", cnpjsDesconhecidos) + ".");

        // O guia obriga o envio de todas as filiais ativas na mesma requisição
        // e proíbe o envio individual.
        var filiaisFaltando = filiaisAtivas
            .Where(b => !filiaisComLinha.Contains(Normalizar(b.Cnpj)))
            .Select(b => b.Cnpj)
            .ToList();

        if (filiaisFaltando.Count > 0)
            preview.Erros.Add(
                "As seguintes filiais ativas não foram encontradas no balancete e o envio individual não é " +
                "permitido: " + string.Join("; ", filiaisFaltando) + ".");

        var itens = agrupado.Values.ToList();
        preview.Itens = itens;
        preview.TotalItens = itens.Count;
        preview.TotalFiliais = filiaisComLinha.Count;

        // Regra 15: a soma das contas de referencial D-IN (1 a 8) tem que ser
        // zero. As contas gerenciais (9xxx) ficam fora dessa conferência.
        preview.TotalValue = itens
            .Where(i => EhContaDeReferencial(i.DfsPk.CodeDFSPlan))
            .Sum(i => i.BalanceValue);

        if (itens.Count > 0 && preview.TotalValue != 0)
            preview.Erros.Add(
                "A soma das contas de Ativo, Passivo e DRE deve ser igual a zero. " +
                $"(Saldo atual: {preview.TotalValue:N2})");

        if (itens.Count > 0)
        {
            preview.TamanhoPayloadBytes = JsonSerializer.SerializeToUtf8Bytes(itens).LongLength;
            if (preview.TamanhoPayloadBytes > TamanhoMaximoBytes)
                preview.Erros.Add(
                    $"O payload tem {preview.TamanhoPayloadBytes / 1024d / 1024d:N2} MB e excede o limite de 4 MB. " +
                    "Divida o envio por período.");
        }

        return preview;
    }

    public async Task<BalanceteEnvioResultado> EnviarAsync(BalanceteRequest request, int userId, string username)
    {
        var resultado = new BalanceteEnvioResultado
        {
            Preview = await ValidarAsync(request)
        };

        if (!resultado.Preview.Valido)
        {
            resultado.Erro = "O balancete não passou nas validações. Corrija os itens apontados e tente novamente.";
            return resultado;
        }

        try
        {
            resultado.Resposta = await _dInService.EnviarAsync(resultado.Preview.Itens, userId, username);
            resultado.Enviado = true;
        }
        catch (Exception ex)
        {
            // O envio (inclusive a falha) já fica registrado em DInReturns pelo DInService.
            resultado.Erro = ex.Message;
        }

        return resultado;
    }

    private static void ValidarPeriodo(BalanceteRequest request, BalancetePreview preview)
    {
        if (!int.TryParse(request.PeriodMonth, out var mes) || mes < 1 || mes > 12)
            preview.Erros.Add("O campo periodMonth precisa estar entre 1 e 12.");

        if (string.IsNullOrWhiteSpace(request.PeriodYear) || request.PeriodYear.Trim().Length != 4
            || !int.TryParse(request.PeriodYear, out _))
            preview.Erros.Add("O campo periodYear precisa ter 4 dígitos.");
    }

    /// <summary>
    /// Contas de referencial D-IN: iniciam de 1 a 8 (Ativo, Passivo e DRE). As
    /// 9xxx são gerenciais e não entram na soma que precisa fechar em zero.
    /// </summary>
    private static bool EhContaDeReferencial(string codeDfsPlan) =>
        !string.IsNullOrEmpty(codeDfsPlan) && codeDfsPlan[0] >= '1' && codeDfsPlan[0] <= '8';

    /// <summary>
    /// Compara códigos e CNPJs ignorando máscara, espaços e caixa, para que
    /// "12.345.678/0001-90" e "12345678000190" sejam a mesma coisa.
    /// </summary>
    private static string Normalizar(string valor)
    {
        if (string.IsNullOrWhiteSpace(valor))
            return string.Empty;

        var chars = valor.Where(c => char.IsLetterOrDigit(c)).Select(char.ToUpperInvariant).ToArray();
        return new string(chars);
    }
}
