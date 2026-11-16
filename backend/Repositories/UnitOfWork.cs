using API.Models;
using Microsoft.EntityFrameworkCore.Storage;

namespace API.Repositories;

public sealed class UnitOfWork(
    ApplicationContext context,
    IGameRepository games,
    ITeamRepository teams,
    IPlayerRepository players,
    ITournamentRepository tournaments,
    ITournamentRegistrationRepository tournamentRegistrations,
    IMatchRepository matches) : IUnitOfWork
{
    public IGameRepository Games { get; } = games;
    public ITeamRepository Teams { get; } = teams;
    public IPlayerRepository Players { get; } = players;
    public ITournamentRepository Tournaments { get; } = tournaments;
    public ITournamentRegistrationRepository TournamentRegistrations { get; } = tournamentRegistrations;
    public IMatchRepository Matches { get; } = matches;

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        context.SaveChangesAsync(cancellationToken);

    public Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default) =>
        context.Database.BeginTransactionAsync(cancellationToken);
}
