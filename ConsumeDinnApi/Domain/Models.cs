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
