using System;
using System.ComponentModel.DataAnnotations;

namespace DIn.Domain;

/// <summary>
/// Perfis de acesso da aplicação. Apenas dois perfis são previstos:
/// Admin (administra usuários) e UserNormal (apenas opera os envios).
/// </summary>
public enum UserRole
{
    Admin = 1,
    UserNormal = 2
}

public class UserLogin
{
    [Key]
    public int Id { get; set; }
    [Required]
    [MaxLength(100)]
    public string Username { get; set; }
    [Required]
    public string PasswordHash { get; set; }
    public UserRole Role { get; set; } = UserRole.UserNormal;
    /// <summary>
    /// Usuário desabilitado não consegue autenticar. Somente Admin altera esse campo.
    /// </summary>
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
}

public class DInReturn
{
    [Key]
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Payload { get; set; }
    public string Response { get; set; }
    public bool Success { get; set; }
    /// <summary>
    /// Usuário que disparou o envio para a API do D-IN.
    /// </summary>
    public int? UserId { get; set; }
    /// <summary>
    /// Username gravado junto ao envio para preservar a autoria mesmo que o
    /// usuário seja removido depois.
    /// </summary>
    [MaxLength(100)]
    public string? Username { get; set; }
    public UserLogin? User { get; set; }
}

public class Department
{
    [Key]
    public int Id { get; set; }
    [Required]
    [MaxLength(50)]
    public string Code { get; set; }
    [Required]
    [MaxLength(200)]
    public string Description { get; set; }
}

public class AccountPlan
{
    [Key]
    public int Id { get; set; }
    [Required]
    [MaxLength(50)]
    public string Code { get; set; }
    [Required]
    [MaxLength(200)]
    public string Description { get; set; }
}

/// <summary>
/// Empresa (grupo) do concessionário. O CNPJ da matriz é o usado como
/// dealerHeadOfficeCode e, conforme o guia, deve ser o cadastrado junto à AGCO
/// (CNPJ do contrato de concessão).
/// </summary>
public class Dealer
{
    [Key]
    public int Id { get; set; }
    [Required]
    [MaxLength(200)]
    public string CorporateName { get; set; } = string.Empty;
    [Required]
    [MaxLength(20)]
    public string HeadOfficeCnpj { get; set; } = string.Empty;
    /// <summary>
    /// Departamento (sectionCode) usado quando a linha do balancete não traz
    /// centro de custo. O guia exige sectionCode em todo item.
    /// </summary>
    [MaxLength(50)]
    public string? DefaultDepartmentCode { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public List<DealerBranch> Branches { get; set; } = new();
}

/// <summary>
/// Filial (dealerBranchCode). O guia obriga o envio de todos os CNPJs ativos
/// ligados à matriz em uma única requisição — o envio individual é bloqueado.
/// Quando a operação tem uma única unidade, o CNPJ da matriz é usado também
/// como filial.
/// </summary>
public class DealerBranch
{
    [Key]
    public int Id { get; set; }
    public int DealerId { get; set; }
    [Required]
    [MaxLength(20)]
    public string Cnpj { get; set; } = string.Empty;
    [MaxLength(200)]
    public string? Name { get; set; }
    /// <summary>Indica a unidade matriz (HeadOffice = BranchCode).</summary>
    public bool IsHeadOffice { get; set; }
    public bool IsActive { get; set; } = true;
    public Dealer? Dealer { get; set; }
}

/// <summary>
/// De/Para de contas: liga a conta contábil do DMS/concessionária ao código do
/// plano de contas do D-IN (codeDFSPlan). O guia exige que toda conta com
/// movimentação e saldo tenha essa relação, e que o envio seja bloqueado
/// quando ela não existir.
/// </summary>
public class AccountMapping
{
    [Key]
    public int Id { get; set; }
    public int DealerId { get; set; }
    /// <summary>Conta na origem (plano de contas da concessionária).</summary>
    [Required]
    [MaxLength(50)]
    public string SourceCode { get; set; } = string.Empty;
    [MaxLength(200)]
    public string? SourceDescription { get; set; }
    /// <summary>Código do plano de contas D-IN (Ex.: 1111 até 8112, mais as 9xxx gerenciais).</summary>
    [Required]
    [MaxLength(50)]
    public string AccountPlanCode { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public Dealer? Dealer { get; set; }
}

/// <summary>
/// De/Para de departamentos: liga o centro de custo da origem ao departamento
/// do D-IN (sectionCode).
/// </summary>
public class DepartmentMapping
{
    [Key]
    public int Id { get; set; }
    public int DealerId { get; set; }
    /// <summary>Centro de custo na origem.</summary>
    [Required]
    [MaxLength(50)]
    public string SourceCode { get; set; } = string.Empty;
    [MaxLength(200)]
    public string? SourceDescription { get; set; }
    /// <summary>Código do departamento D-IN (1101, 1102, 1201, 1301, 1401 ou 1501).</summary>
    [Required]
    [MaxLength(50)]
    public string DepartmentCode { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public Dealer? Dealer { get; set; }
}
