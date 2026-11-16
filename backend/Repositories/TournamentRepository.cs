using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories;

public sealed class TournamentRepository(ApplicationContext context) : ITournamentRepository
{
    public async Task<Tournament?> GetByIdAsync(int id, bool includeRegistrations = false, bool includeMatches = false, CancellationToken cancellationToken = default)
    {
        IQueryable<Tournament> query = context.Tournaments.AsNoTracking();
        if (includeRegistrations)
            query = query.Include(tournament => tournament.TournamentRegistrations)
                .ThenInclude(registration => registration.Team);
        if (includeMatches)
            query = query.Include(tournament => tournament.Matches);
        return await query.SingleOrDefaultAsync(tournament => tournament.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Tournament>> GetPagedAsync(int page, int pageSize, int? gameId = null, string? search = null, CancellationToken cancellationToken = default)
    {
        IQueryable<Tournament> query = context.Tournaments.AsNoTracking();
        if (gameId.HasValue)
            query = query.Where(tournament => tournament.GameId == gameId.Value);
        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(tournament => tournament.Name.Contains(search));
        return await query.OrderByDescending(tournament => tournament.StartDate)
            .Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);
    }

    public Task<int> CountAsync(int? gameId = null, string? search = null, CancellationToken cancellationToken = default)
    {
        IQueryable<Tournament> query = context.Tournaments;
        if (gameId.HasValue)
            query = query.Where(tournament => tournament.GameId == gameId.Value);
        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(tournament => tournament.Name.Contains(search));
        return query.CountAsync(cancellationToken);
    }

    public Task AddAsync(Tournament tournament, CancellationToken cancellationToken = default) =>
        context.Tournaments.AddAsync(tournament, cancellationToken).AsTask();

    public void Update(Tournament tournament) => context.Tournaments.Update(tournament);
    public void Delete(Tournament tournament) => context.Tournaments.Remove(tournament);
}
