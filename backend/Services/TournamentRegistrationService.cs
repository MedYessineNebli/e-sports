using API.DTOs;
using API.Repositories;

namespace API.Services;

public sealed class TournamentRegistrationService(IUnitOfWork unitOfWork) : ITournamentRegistrationService
{
    public async Task<PagedResult<RegistrationResponseDto>> GetPagedAsync(PagedRequest request, int? tournamentId, int? teamId, CancellationToken cancellationToken)
    {
        var page = request.ValidatedPage;
        var pageSize = request.ValidatedPageSize;
        var totalCount = await unitOfWork.TournamentRegistrations.CountAsync(tournamentId, teamId, cancellationToken);
        var items = await unitOfWork.TournamentRegistrations.GetPagedAsync(page, pageSize, tournamentId, teamId, cancellationToken);
        return new(page, pageSize, totalCount, GetTotalPages(totalCount, pageSize), items.Select(ToResponse).ToList());
    }

    public async Task<RegistrationResponseDto?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        var registration = await unitOfWork.TournamentRegistrations.GetByIdAsync(id, true, true, cancellationToken);
        return registration is null ? null : ToResponse(registration);
    }

    public async Task<RegistrationResponseDto> CreateAsync(CreateRegistrationDto dto, CancellationToken cancellationToken)
    {
        var tournament = await unitOfWork.Tournaments.GetByIdAsync(dto.TournamentId, cancellationToken: cancellationToken)
            ?? throw new KeyNotFoundException("Le tournoi demandé n'existe pas.");
        var team = await unitOfWork.Teams.GetByIdAsync(dto.TeamId, cancellationToken: cancellationToken)
            ?? throw new KeyNotFoundException("L'équipe demandée n'existe pas.");
        if (await unitOfWork.TournamentRegistrations.ExistsAsync(dto.TournamentId, dto.TeamId, cancellationToken))
            throw new InvalidOperationException("Cette équipe est déjà inscrite à ce tournoi.");

        var registration = new TournamentRegistration
        {
            TournamentId = tournament.Id,
            Tournament = tournament,
            TeamId = team.Id,
            Team = team,
            RegisteredAt = DateTime.UtcNow
        };
        await unitOfWork.TournamentRegistrations.AddAsync(registration, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return ToResponse(registration);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var registration = await unitOfWork.TournamentRegistrations.GetByIdAsync(id, cancellationToken: cancellationToken);
        if (registration is null)
            return false;
        unitOfWork.TournamentRegistrations.Delete(registration);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static RegistrationResponseDto ToResponse(TournamentRegistration registration) =>
        new(registration.Id, registration.TournamentId, registration.Tournament?.Name ?? string.Empty,
            registration.TeamId, registration.Team?.Name ?? string.Empty, registration.RegisteredAt);
    private static int GetTotalPages(int totalCount, int pageSize) => (int)Math.Ceiling(totalCount / (double)pageSize);
}
