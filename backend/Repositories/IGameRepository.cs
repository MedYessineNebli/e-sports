public interface IGameRepository
{
    Task<Game?> GetByIdAsync(int id, bool includeTournaments = false, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Game>> GetPagedAsync(int page, int pageSize, string? search = null, CancellationToken cancellationToken = default);
    Task<int> CountAsync(string? search = null, CancellationToken cancellationToken = default);
    Task AddAsync(Game game, CancellationToken cancellationToken = default);
    void Update(Game game);
    void Delete(Game game);
}
