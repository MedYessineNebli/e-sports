using System.ComponentModel.DataAnnotations;

namespace API.DTOs;

public sealed record CreateRegistrationDto
{
    [Range(1, int.MaxValue)]
    public int TournamentId { get; init; }

    [Range(1, int.MaxValue)]
    public int TeamId { get; init; }
}

public sealed record RegistrationResponseDto(
    int Id,
    int TournamentId,
    string TournamentName,
    int TeamId,
    string TeamName,
    DateTime RegisteredAt);
