namespace API.DTOs;

public sealed record StandingResponseDto(
    int TeamId,
    string TeamName,
    int Played,
    int Wins,
    int Losses,
    int Draws,
    int Points);
