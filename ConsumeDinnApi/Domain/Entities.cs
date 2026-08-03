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
