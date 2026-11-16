using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories;

public sealed class GameRepository(ApplicationContext context) : IGameRepository
{
    public async Task<Game?> GetByIdAsync(int id, bool includeTournaments = false, CancellationToken cancellationToken = default)
    {
        IQueryable<Game> query = context.Games.AsNoTracking();
        if (includeTournaments)
            query = query.Include(game => game.Tournaments);
        return await query.SingleOrDefaultAsync(game => game.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Game>> GetPagedAsync(int page, int pageSize, string? search = null, CancellationToken cancellationToken = default)
    {
        IQueryable<Game> query = context.Games.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(game => game.Name.Contains(search));
        return await query.OrderBy(game => game.Name)
            .Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);
    }

    public Task<int> CountAsync(string? search = null, CancellationToken cancellationToken = default)
    {
        IQueryable<Game> query = context.Games;
        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(game => game.Name.Contains(search));
        return query.CountAsync(cancellationToken);
    }

    public Task AddAsync(Game game, CancellationToken cancellationToken = default) =>
        context.Games.AddAsync(game, cancellationToken).AsTask();

    public void Update(Game game) => context.Games.Update(game);
    public void Delete(Game game) => context.Games.Remove(game);
}
