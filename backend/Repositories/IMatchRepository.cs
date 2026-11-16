public interface IMatchRepository
{
    Task<Match?> GetByIdAsync(int id, bool includeTeams = false, bool includeTournament = false, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Match>> GetPagedAsync(int page, int pageSize, int? tournamentId = null, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Match>> GetCompletedPagedAsync(int page, int pageSize, int? tournamentId = null, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Match>> GetCompletedByTournamentAsync(int tournamentId, CancellationToken cancellationToken = default);
    Task<int> CountAsync(int? tournamentId = null, CancellationToken cancellationToken = default);
    Task<int> CountCompletedAsync(int? tournamentId = null, CancellationToken cancellationToken = default);
    Task AddAsync(Match match, CancellationToken cancellationToken = default);
    void Update(Match match);
    void Delete(Match match);
}
