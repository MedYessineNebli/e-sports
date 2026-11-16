using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories;

public sealed class MatchRepository(ApplicationContext context) : IMatchRepository
{
    public async Task<Match?> GetByIdAsync(int id, bool includeTeams = false, bool includeTournament = false, CancellationToken cancellationToken = default)
    {
        IQueryable<Match> query = context.Matches.AsNoTracking();
        if (includeTeams)
        {
            query = query.Include(match => match.TeamA)
                .Include(match => match.TeamB)
                .Include(match => match.WinnerTeam);
        }
        if (includeTournament)
            query = query.Include(match => match.Tournament);
        return await query.SingleOrDefaultAsync(match => match.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Match>> GetPagedAsync(int page, int pageSize, int? tournamentId = null, CancellationToken cancellationToken = default)
    {
        IQueryable<Match> query = context.Matches.AsNoTracking()
            .Include(match => match.TeamA)
            .Include(match => match.TeamB)
            .Include(match => match.WinnerTeam);
        if (tournamentId.HasValue)
            query = query.Where(match => match.TournamentId == tournamentId.Value);
        return await query.OrderBy(match => match.ScheduledAt)
            .Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Match>> GetCompletedByTournamentAsync(int tournamentId, CancellationToken cancellationToken = default) =>
        await context.Matches.AsNoTracking()
            .Include(match => match.TeamA)
            .Include(match => match.TeamB)
            .Where(match => match.TournamentId == tournamentId && match.PlayedAt != null)
            .OrderBy(match => match.ScheduledAt)
            .ToListAsync(cancellationToken);

    public Task<int> CountAsync(int? tournamentId = null, CancellationToken cancellationToken = default)
    {
        IQueryable<Match> query = context.Matches;
        if (tournamentId.HasValue)
            query = query.Where(match => match.TournamentId == tournamentId.Value);
        return query.CountAsync(cancellationToken);
    }

    public Task AddAsync(Match match, CancellationToken cancellationToken = default) =>
        context.Matches.AddAsync(match, cancellationToken).AsTask();

    public void Update(Match match) => context.Matches.Update(match);
    public void Delete(Match match) => context.Matches.Remove(match);
}
