public interface IPlayerRepository
{
    Task<Player?> GetByIdAsync(int id, bool includeTeam = false, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Player>> GetPagedAsync(int page, int pageSize, int? teamId = null, string? search = null, CancellationToken cancellationToken = default);
    Task<int> CountAsync(int? teamId = null, string? search = null, CancellationToken cancellationToken = default);
    Task AddAsync(Player player, CancellationToken cancellationToken = default);
    void Update(Player player);
    void Delete(Player player);
}
