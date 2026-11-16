public interface ITournamentRepository
{
    Task<Tournament?> GetByIdAsync(int id, bool includeRegistrations = false, bool includeMatches = false, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Tournament>> GetPagedAsync(int page, int pageSize, int? gameId = null, string? search = null, CancellationToken cancellationToken = default);
    Task<int> CountAsync(int? gameId = null, string? search = null, CancellationToken cancellationToken = default);
    Task AddAsync(Tournament tournament, CancellationToken cancellationToken = default);
    void Update(Tournament tournament);
    void Delete(Tournament tournament);
}
