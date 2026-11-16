public interface ITeamRepository
{
    Task<Team?> GetByIdAsync(int id, bool includePlayers = false, bool includeRegistrations = false, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Team>> GetPagedAsync(int page, int pageSize, string? search = null, CancellationToken cancellationToken = default);
    Task<int> CountAsync(string? search = null, CancellationToken cancellationToken = default);
    Task AddAsync(Team team, CancellationToken cancellationToken = default);
    void Update(Team team);
    void Delete(Team team);
}
