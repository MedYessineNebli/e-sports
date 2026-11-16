using System.ComponentModel.DataAnnotations;

namespace API.DTOs;

public sealed record CreatePlayerDto
{
    [Required, MaxLength(100)]
    public required string Gamertag { get; init; }

    [Range(1, int.MaxValue)]
    public int TeamId { get; init; }
}

public sealed record UpdatePlayerDto
{
    [Required, MaxLength(100)]
    public required string Gamertag { get; init; }

    [Range(1, int.MaxValue)]
    public int TeamId { get; init; }
}

public sealed record PlayerResponseDto(
    int Id,
    string Gamertag,
    int TeamId,
    string? TeamName);
