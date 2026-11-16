using API.DTOs;
using MediatR;

namespace API.CQRS.Tournaments;

public sealed record GetTournamentStandingsQuery(int TournamentId)
    : IRequest<IReadOnlyList<StandingResponseDto>?>;
