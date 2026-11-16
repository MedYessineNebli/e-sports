using System.ComponentModel.DataAnnotations;

namespace API.DTOs;

public sealed record CreateGameDto
{
    [Required, MaxLength(100)]
    public required string Name { get; init; }

    [MaxLength(1000)]
    public string? Description { get; init; }
}

public sealed record UpdateGameDto
{
    [Required, MaxLength(100)]
    public required string Name { get; init; }

    [MaxLength(1000)]
    public string? Description { get; init; }
}

public sealed record GameResponseDto(
    int Id,
    string Name,
    string? Description);
