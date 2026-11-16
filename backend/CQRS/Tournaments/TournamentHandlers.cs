using API.DTOs;
using API.Services;
using MediatR;

namespace API.CQRS.Tournaments;

public sealed class GetTournamentStandingsHandler(ITournamentService service)
    : IRequestHandler<GetTournamentStandingsQuery, IReadOnlyList<StandingResponseDto>?>
{
    public Task<IReadOnlyList<StandingResponseDto>?> Handle(
        GetTournamentStandingsQuery request,
        CancellationToken cancellationToken) =>
        service.GetStandingsAsync(request.TournamentId, cancellationToken);
}
