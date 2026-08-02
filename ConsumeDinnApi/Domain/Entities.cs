using System;
using System.ComponentModel.DataAnnotations;

namespace DIn.Domain;

public class UserLogin
{
    [Key]
    public int Id { get; set; }
    [Required]
    [MaxLength(100)]
    public string Username { get; set; }
    [Required]
    public string PasswordHash { get; set; }
}

public class DInReturn
{
    [Key]
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Payload { get; set; }
    public string Response { get; set; }
    public bool Success { get; set; }
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
