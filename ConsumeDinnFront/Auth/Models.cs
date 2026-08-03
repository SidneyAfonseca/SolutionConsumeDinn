namespace ConsumeDinnFront.Auth;

/// <summary>
/// Perfis de acesso — espelha DIn.Domain.UserRole no backend.
/// </summary>
public enum UserRole
{
    Admin = 1,
    UserNormal = 2
}

public class LoginRequest
{
    public string Username { get; set; } = string.Empty;
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
    public int TotalEnvios { get; set; }
    public DateTime? UltimoEnvio { get; set; }
}

public class CreateUserRequest
{
    public string Username { get; set; } = string.Empty;
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
    public string Password { get; set; } = string.Empty;
}

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

/// <summary>Mensagem de erro devolvida pela API.</summary>
public class ApiError
{
    public string? Message { get; set; }
}

// ---------------------------------------------------------------------------
// Empresa, De/Para e balancete
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
    public string CorporateName { get; set; } = string.Empty;
    public string HeadOfficeCnpj { get; set; } = string.Empty;
    public string? DefaultDepartmentCode { get; set; }
    public bool IsActive { get; set; } = true;
}

public class SaveBranchRequest
{
    public string Cnpj { get; set; } = string.Empty;
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
    public string? AccountPlanDescription { get; set; }
    public bool IsActive { get; set; }
}

public class SaveAccountMappingRequest
{
    public string SourceCode { get; set; } = string.Empty;
    public string? SourceDescription { get; set; }
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
    public string SourceCode { get; set; } = string.Empty;
    public string? SourceDescription { get; set; }
    public string DepartmentCode { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
}

public class CodeDescriptionDto
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public class BalanceteLinha
{
    public string? BranchCnpj { get; set; }
    public string SourceAccountCode { get; set; } = string.Empty;
    public string? SourceCostCenter { get; set; }
    public decimal BalanceValue { get; set; }
    public int Numero { get; set; }
}

public class BalanceteRequest
{
    public int DealerId { get; set; }
    public string PeriodMonth { get; set; } = string.Empty;
    public string PeriodYear { get; set; } = string.Empty;
    public List<BalanceteLinha> Linhas { get; set; } = new();
}

public class DfsPkDto
{
    public string CodeDFSPlan { get; set; } = string.Empty;
    public string DealerBranchCode { get; set; } = string.Empty;
    public string PeriodMonth { get; set; } = string.Empty;
    public string PeriodYear { get; set; } = string.Empty;
    public string SectionCode { get; set; } = string.Empty;
}

public class DInRequestDto
{
    public decimal BalanceValue { get; set; }
    public string DealerHeadOfficeCode { get; set; } = string.Empty;
    public DfsPkDto DfsPk { get; set; } = new();
}

public class DInResponseDto
{
    public List<string>? DealerBranchCodes { get; set; }
    public string? DealerHeadOfficeCode { get; set; }
    public string? PeriodMonth { get; set; }
    public string? PeriodYear { get; set; }
    public string? Protocol { get; set; }
    public decimal TotalValue { get; set; }
}

public class BalancetePreview
{
    public bool Valido { get; set; }
    public List<string> Erros { get; set; } = new();
    public List<string> Avisos { get; set; } = new();
    public decimal TotalValue { get; set; }
    public int TotalItens { get; set; }
    public int TotalLinhasOrigem { get; set; }
    public int TotalFiliais { get; set; }
    public long TamanhoPayloadBytes { get; set; }
    public List<string> ContasSemDePara { get; set; } = new();
    public List<string> DepartamentosSemDePara { get; set; } = new();
    public List<DInRequestDto> Itens { get; set; } = new();
}

public class BalanceteEnvioResultado
{
    public BalancetePreview Preview { get; set; } = new();
    public DInResponseDto? Resposta { get; set; }
    public bool Enviado { get; set; }
    public string? Erro { get; set; }
}
