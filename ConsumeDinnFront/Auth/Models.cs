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
