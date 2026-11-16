using System.ComponentModel.DataAnnotations;

namespace API.DTOs;

public sealed record CreateTournamentDto
{
    [Required, MaxLength(150)]
    public required string Name { get; init; }

    [Range(1, int.MaxValue)]
    public int GameId { get; init; }

    public DateTime StartDate { get; init; }
    public DateTime EndDate { get; init; }
}

public sealed record UpdateTournamentDto
{
    [Required, MaxLength(150)]
    public required string Name { get; init; }

    [Range(1, int.MaxValue)]
    public int GameId { get; init; }

    public DateTime StartDate { get; init; }
    public DateTime EndDate { get; init; }
}

public sealed record TournamentResponseDto(
    int Id,
    string Name,
    int GameId,
    DateTime StartDate,
    DateTime EndDate);
