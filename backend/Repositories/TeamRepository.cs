using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories;

public sealed class TeamRepository(ApplicationContext context) : ITeamRepository
{
    public async Task<Team?> GetByIdAsync(int id, bool includePlayers = false, bool includeRegistrations = false, CancellationToken cancellationToken = default)
    {
        IQueryable<Team> query = context.Teams.AsNoTracking();
        if (includePlayers)
            query = query.Include(team => team.Players);
        if (includeRegistrations)
            query = query.Include(team => team.TournamentRegistrations);
        return await query.SingleOrDefaultAsync(team => team.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Team>> GetPagedAsync(int page, int pageSize, string? search = null, CancellationToken cancellationToken = default)
    {
        IQueryable<Team> query = context.Teams.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(team => team.Name.Contains(search));
        return await query.OrderBy(team => team.Name)
            .Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);
    }

    public Task<int> CountAsync(string? search = null, CancellationToken cancellationToken = default)
    {
        IQueryable<Team> query = context.Teams;
        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(team => team.Name.Contains(search));
        return query.CountAsync(cancellationToken);
    }

    public Task AddAsync(Team team, CancellationToken cancellationToken = default) =>
        context.Teams.AddAsync(team, cancellationToken).AsTask();

    public void Update(Team team) => context.Teams.Update(team);
    public void Delete(Team team) => context.Teams.Remove(team);
}
