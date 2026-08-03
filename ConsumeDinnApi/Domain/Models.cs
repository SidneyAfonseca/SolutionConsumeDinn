using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DIn.Domain;

public class DInRequest
{
    public decimal BalanceValue { get; set; }
    public string DealerHeadOfficeCode { get; set; }
    public DfsPk DfsPk { get; set; }
}

public class DfsPk
{
    public string CodeDFSPlan { get; set; }
    public string DealerBranchCode { get; set; }
    public string PeriodMonth { get; set; }
    public string PeriodYear { get; set; }
    public string SectionCode { get; set; }
}

public class DInResponse
{
    public List<string> DealerBranchCodes { get; set; }
    public string DealerHeadOfficeCode { get; set; }
    public string PeriodMonth { get; set; }
    public string PeriodYear { get; set; }
    public string Protocol { get; set; }
    public decimal TotalValue { get; set; }
}

// ---------------------------------------------------------------------------
// Autenticação e administração de usuários
// ---------------------------------------------------------------------------

public class LoginRequest
{
    [Required(ErrorMessage = "Informe o usuário")]
    public string Username { get; set; } = string.Empty;
    [Required(ErrorMessage = "Informe a senha")]
    public string Password { get; set; } = string.Empty;
}

public class LoginResponse
{
    public string Token { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
}

public class UserDto
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    /// <summary>Quantidade de envios ao D-IN realizados pelo usuário.</summary>
    public int TotalEnvios { get; set; }
    public DateTime? UltimoEnvio { get; set; }
}

public class CreateUserRequest
{
    [Required(ErrorMessage = "Informe o usuário")]
    [MaxLength(100)]
    public string Username { get; set; } = string.Empty;
    [Required(ErrorMessage = "Informe a senha")]
    [MinLength(6, ErrorMessage = "A senha deve ter no mínimo 6 caracteres")]
    public string Password { get; set; } = string.Empty;
    public UserRole Role { get; set; } = UserRole.UserNormal;
}

public class UpdateUserStatusRequest
{
    public bool IsActive { get; set; }
}

public class UpdateUserRoleRequest
{
    public UserRole Role { get; set; }
}

public class ResetPasswordRequest
{
    [Required(ErrorMessage = "Informe a senha")]
    [MinLength(6, ErrorMessage = "A senha deve ter no mínimo 6 caracteres")]
    public string Password { get; set; } = string.Empty;
}

// ---------------------------------------------------------------------------
// Cadastro da empresa e De/Para
// ---------------------------------------------------------------------------

public class DealerDto
{
    public int Id { get; set; }
    public string CorporateName { get; set; } = string.Empty;
    public string HeadOfficeCnpj { get; set; } = string.Empty;
    public string? DefaultDepartmentCode { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<DealerBranchDto> Branches { get; set; } = new();
    public int TotalContasDePara { get; set; }
    public int TotalDepartamentosDePara { get; set; }
}

public class DealerBranchDto
{
    public int Id { get; set; }
    public string Cnpj { get; set; } = string.Empty;
    public string? Name { get; set; }
    public bool IsHeadOffice { get; set; }
    public bool IsActive { get; set; }
}

public class SaveDealerRequest
{
    [Required(ErrorMessage = "Informe a razão social")]
    [MaxLength(200)]
    public string CorporateName { get; set; } = string.Empty;
    [Required(ErrorMessage = "Informe o CNPJ da matriz")]
    [MaxLength(20)]
    public string HeadOfficeCnpj { get; set; } = string.Empty;
    [MaxLength(50)]
    public string? DefaultDepartmentCode { get; set; }
    public bool IsActive { get; set; } = true;
}

public class SaveBranchRequest
{
    [Required(ErrorMessage = "Informe o CNPJ da filial")]
    [MaxLength(20)]
    public string Cnpj { get; set; } = string.Empty;
    [MaxLength(200)]
    public string? Name { get; set; }
    public bool IsHeadOffice { get; set; }
    public bool IsActive { get; set; } = true;
}

public class AccountMappingDto
{
    public int Id { get; set; }
    public string SourceCode { get; set; } = string.Empty;
    public string? SourceDescription { get; set; }
    public string AccountPlanCode { get; set; } = string.Empty;
    /// <summary>Descrição da conta D-IN, para conferência na tela.</summary>
    public string? AccountPlanDescription { get; set; }
    public bool IsActive { get; set; }
}

public class SaveAccountMappingRequest
{
    [Required(ErrorMessage = "Informe a conta de origem")]
    [MaxLength(50)]
    public string SourceCode { get; set; } = string.Empty;
    [MaxLength(200)]
    public string? SourceDescription { get; set; }
    [Required(ErrorMessage = "Informe o código do plano de contas D-IN")]
    [MaxLength(50)]
    public string AccountPlanCode { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
}

public class DepartmentMappingDto
{
    public int Id { get; set; }
    public string SourceCode { get; set; } = string.Empty;
    public string? SourceDescription { get; set; }
    public string DepartmentCode { get; set; } = string.Empty;
    public string? DepartmentDescription { get; set; }
    public bool IsActive { get; set; }
}

public class SaveDepartmentMappingRequest
{
    [Required(ErrorMessage = "Informe o centro de custo de origem")]
    [MaxLength(50)]
    public string SourceCode { get; set; } = string.Empty;
    [MaxLength(200)]
    public string? SourceDescription { get; set; }
    [Required(ErrorMessage = "Informe o código do departamento D-IN")]
    [MaxLength(50)]
    public string DepartmentCode { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
}

// ---------------------------------------------------------------------------
// Envio do balancete
// ---------------------------------------------------------------------------

/// <summary>
/// Linha do balancete na origem, antes do De/Para.
/// </summary>
public class BalanceteLinha
{
    /// <summary>CNPJ da filial. Vazio assume a matriz.</summary>
    public string? BranchCnpj { get; set; }
    /// <summary>Conta no plano de contas da concessionária.</summary>
    public string SourceAccountCode { get; set; } = string.Empty;
    /// <summary>Centro de custo na origem. Vazio usa o departamento padrão da empresa.</summary>
    public string? SourceCostCenter { get; set; }
    public decimal BalanceValue { get; set; }
    /// <summary>Número da linha no arquivo/texto, usado nas mensagens de erro.</summary>
    public int Numero { get; set; }
}

public class BalanceteRequest
{
    public int DealerId { get; set; }
    [Required(ErrorMessage = "Informe o mês do período")]
    public string PeriodMonth { get; set; } = string.Empty;
    [Required(ErrorMessage = "Informe o ano do período")]
    public string PeriodYear { get; set; } = string.Empty;
    public List<BalanceteLinha> Linhas { get; set; } = new();
}

/// <summary>
/// Resultado da validação do balancete: o payload pronto para a AGCO, os
/// totais e a lista de problemas encontrados (com a linha de origem).
/// </summary>
public class BalancetePreview
{
    public bool Valido => Erros.Count == 0;
    public List<string> Erros { get; set; } = new();
    public List<string> Avisos { get; set; } = new();
    /// <summary>Soma das contas de referencial D-IN (1 a 8) — precisa ser zero.</summary>
    public decimal TotalValue { get; set; }
    public int TotalItens { get; set; }
    public int TotalLinhasOrigem { get; set; }
    public int TotalFiliais { get; set; }
    public long TamanhoPayloadBytes { get; set; }
    /// <summary>Contas de origem sem De/Para, que impedem o envio.</summary>
    public List<string> ContasSemDePara { get; set; } = new();
    /// <summary>Centros de custo sem De/Para, que impedem o envio.</summary>
    public List<string> DepartamentosSemDePara { get; set; } = new();
    public List<DInRequest> Itens { get; set; } = new();
}

public class BalanceteEnvioResultado
{
    public BalancetePreview Preview { get; set; } = new();
    public DInResponse? Resposta { get; set; }
    public bool Enviado { get; set; }
    public string? Erro { get; set; }
}

/// <summary>
/// Registro de um envio ao D-IN, com a autoria do usuário que o disparou.
/// </summary>
public class DInReturnDto
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public int? UserId { get; set; }
    public string? Username { get; set; }
    public bool Success { get; set; }
    public string Response { get; set; } = string.Empty;
    public string Payload { get; set; } = string.Empty;
}
