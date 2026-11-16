using System.ComponentModel.DataAnnotations;

namespace API.DTOs;

public sealed record CreateTeamDto
{
    [Required, MaxLength(100)]
    public required string Name { get; init; }
}

public sealed record UpdateTeamDto
{
    [Required, MaxLength(100)]
    public required string Name { get; init; }
}

public sealed record TeamResponseDto(
    int Id,
    string Name);
