using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories;

public sealed class PlayerRepository(ApplicationContext context) : IPlayerRepository
{
    public async Task<Player?> GetByIdAsync(int id, bool includeTeam = false, CancellationToken cancellationToken = default)
    {
        IQueryable<Player> query = context.Players.AsNoTracking();
        if (includeTeam)
            query = query.Include(player => player.Team);
        return await query.SingleOrDefaultAsync(player => player.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Player>> GetPagedAsync(int page, int pageSize, int? teamId = null, string? search = null, CancellationToken cancellationToken = default)
    {
        IQueryable<Player> query = context.Players.AsNoTracking();
        if (teamId.HasValue)
            query = query.Where(player => player.TeamId == teamId.Value);
        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(player => player.Gamertag.Contains(search));
        return await query.OrderBy(player => player.Gamertag)
            .Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);
    }

    public Task<int> CountAsync(int? teamId = null, string? search = null, CancellationToken cancellationToken = default)
    {
        IQueryable<Player> query = context.Players;
        if (teamId.HasValue)
            query = query.Where(player => player.TeamId == teamId.Value);
        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(player => player.Gamertag.Contains(search));
        return query.CountAsync(cancellationToken);
    }

    public Task AddAsync(Player player, CancellationToken cancellationToken = default) =>
        context.Players.AddAsync(player, cancellationToken).AsTask();

    public void Update(Player player) => context.Players.Update(player);
    public void Delete(Player player) => context.Players.Remove(player);
}
