using System.ComponentModel.DataAnnotations;

namespace API.DTOs;

public sealed record RegisterDto
{
    [Required, EmailAddress]
    public required string Email { get; init; }

    [Required, MaxLength(100)]
    public required string UserName { get; init; }

    [Required, MinLength(8)]
    public required string Password { get; init; }

    [Required, Compare(nameof(Password))]
    public required string ConfirmPassword { get; init; }
}

public sealed record LoginResponseDto(
    string Token,
    DateTime Expiration,
    string UserId,
    string Email,
    IReadOnlyList<string> Roles);

public sealed record CreateRoleDto
{
    [Required, MaxLength(50)]
    public required string Name { get; init; }
}

public sealed record AssignRoleDto
{
    [Required]
    public required string UserId { get; init; }

    [Required, MaxLength(50)]
    public required string RoleName { get; init; }
}

/// <summary>Represents a user managed by an administrator.</summary>
public sealed record UserResponse(
    string Id,
    string UserName,
    string Email,
    IReadOnlyList<string> Roles);

/// <summary>Payload for creating a user.</summary>
public sealed record CreateUserDto
{
    [Required, MaxLength(100)]
    public required string UserName { get; init; }

    [Required, EmailAddress]
    public required string Email { get; init; }

    [Required, MinLength(8)]
    public required string Password { get; init; }

    [Required, MaxLength(50)]
    public required string RoleName { get; init; }
}

/// <summary>Payload for updating a user.</summary>
public sealed record UpdateUserDto
{
    [Required, MaxLength(100)]
    public required string UserName { get; init; }

    [Required, EmailAddress]
    public required string Email { get; init; }

    [Required, MaxLength(50)]
    public required string RoleName { get; init; }

    [MinLength(8)]
    public string? Password { get; init; }
}
