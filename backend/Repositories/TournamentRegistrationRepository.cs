using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories;

public sealed class TournamentRegistrationRepository(ApplicationContext context) : ITournamentRegistrationRepository
{
    public async Task<TournamentRegistration?> GetByIdAsync(int id, bool includeTeam = false, bool includeTournament = false, CancellationToken cancellationToken = default)
    {
        IQueryable<TournamentRegistration> query = context.TournamentRegistrations.AsNoTracking();
        if (includeTeam)
            query = query.Include(registration => registration.Team);
        if (includeTournament)
            query = query.Include(registration => registration.Tournament);
        return await query.SingleOrDefaultAsync(registration => registration.Id == id, cancellationToken);
    }

    public Task<bool> ExistsAsync(int tournamentId, int teamId, CancellationToken cancellationToken = default) =>
        context.TournamentRegistrations.AnyAsync(
            registration => registration.TournamentId == tournamentId && registration.TeamId == teamId,
            cancellationToken);

    public async Task<IReadOnlyList<TournamentRegistration>> GetPagedAsync(int page, int pageSize, int? tournamentId = null, int? teamId = null, CancellationToken cancellationToken = default)
    {
        IQueryable<TournamentRegistration> query = context.TournamentRegistrations.AsNoTracking()
            .Include(registration => registration.Team)
            .Include(registration => registration.Tournament);
        if (tournamentId.HasValue)
            query = query.Where(registration => registration.TournamentId == tournamentId.Value);
        if (teamId.HasValue)
            query = query.Where(registration => registration.TeamId == teamId.Value);
        return await query.OrderByDescending(registration => registration.RegisteredAt)
            .Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);
    }

    public Task<int> CountAsync(int? tournamentId = null, int? teamId = null, CancellationToken cancellationToken = default)
    {
        IQueryable<TournamentRegistration> query = context.TournamentRegistrations;
        if (tournamentId.HasValue)
            query = query.Where(registration => registration.TournamentId == tournamentId.Value);
        if (teamId.HasValue)
            query = query.Where(registration => registration.TeamId == teamId.Value);
        return query.CountAsync(cancellationToken);
    }

    public Task AddAsync(TournamentRegistration registration, CancellationToken cancellationToken = default) =>
        context.TournamentRegistrations.AddAsync(registration, cancellationToken).AsTask();

    public void Update(TournamentRegistration registration) => context.TournamentRegistrations.Update(registration);
    public void Delete(TournamentRegistration registration) => context.TournamentRegistrations.Remove(registration);
}
