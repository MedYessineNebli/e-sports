public interface ITournamentRegistrationRepository
{
    Task<TournamentRegistration?> GetByIdAsync(int id, bool includeTeam = false, bool includeTournament = false, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(int tournamentId, int teamId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TournamentRegistration>> GetPagedAsync(int page, int pageSize, int? tournamentId = null, int? teamId = null, CancellationToken cancellationToken = default);
    Task<int> CountAsync(int? tournamentId = null, int? teamId = null, CancellationToken cancellationToken = default);
    Task AddAsync(TournamentRegistration registration, CancellationToken cancellationToken = default);
    void Update(TournamentRegistration registration);
    void Delete(TournamentRegistration registration);
}
