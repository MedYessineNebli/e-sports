using System.ComponentModel.DataAnnotations;

namespace API.DTOs;

public sealed record CreateMatchDto
{
    [Range(1, int.MaxValue)]
    public int TournamentId { get; init; }

    [Range(1, int.MaxValue)]
    public int TeamAId { get; init; }

    [Range(1, int.MaxValue)]
    public int TeamBId { get; init; }

    public DateTime ScheduledAt { get; init; }
}

public sealed record UpdateMatchScoreDto
{
    [Range(0, int.MaxValue)]
    public int ScoreTeamA { get; init; }

    [Range(0, int.MaxValue)]
    public int ScoreTeamB { get; init; }

    public DateTime? PlayedAt { get; init; }
}

public sealed record MatchResponseDto(
    int Id,
    int TournamentId,
    int TeamAId,
    string TeamAName,
    int TeamBId,
    string TeamBName,
    int? WinnerTeamId,
    int ScoreTeamA,
    int ScoreTeamB,
    DateTime ScheduledAt,
    DateTime? PlayedAt);
