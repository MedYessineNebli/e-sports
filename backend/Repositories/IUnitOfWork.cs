using Microsoft.EntityFrameworkCore.Storage;

namespace API.Repositories;

public interface IUnitOfWork
{
    IGameRepository Games { get; }
    ITeamRepository Teams { get; }
    IPlayerRepository Players { get; }
    ITournamentRepository Tournaments { get; }
    ITournamentRegistrationRepository TournamentRegistrations { get; }
    IMatchRepository Matches { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);
}
