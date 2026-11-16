using API.DTOs;
using API.Repositories;

namespace API.Services;

public sealed class MatchService(IUnitOfWork unitOfWork) : IMatchService
{
    public async Task<PagedResult<MatchResponseDto>> GetPagedAsync(PagedRequest request, int? tournamentId, CancellationToken cancellationToken)
    {
        var page = request.ValidatedPage;
        var pageSize = request.ValidatedPageSize;
        var totalCount = await unitOfWork.Matches.CountAsync(tournamentId, cancellationToken);
        var items = await unitOfWork.Matches.GetPagedAsync(page, pageSize, tournamentId, cancellationToken);
        return new(page, pageSize, totalCount, GetTotalPages(totalCount, pageSize), items.Select(ToResponse).ToList());
    }

    public async Task<MatchResponseDto?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        var match = await unitOfWork.Matches.GetByIdAsync(id, true, true, cancellationToken);
        return match is null ? null : ToResponse(match);
    }

    public async Task<MatchResponseDto> CreateAsync(CreateMatchDto dto, CancellationToken cancellationToken)
    {
        if (dto.TeamAId == dto.TeamBId)
            throw new ArgumentException("Les deux équipes d'un match doivent être différentes.");
        if (dto.ScheduledAt == default)
            throw new ArgumentException("La date du match est obligatoire.");
        _ = await unitOfWork.Tournaments.GetByIdAsync(dto.TournamentId, cancellationToken: cancellationToken)
            ?? throw new KeyNotFoundException("Le tournoi demandé n'existe pas.");
        _ = await unitOfWork.Teams.GetByIdAsync(dto.TeamAId, cancellationToken: cancellationToken)
            ?? throw new KeyNotFoundException("L'équipe A demandée n'existe pas.");
        _ = await unitOfWork.Teams.GetByIdAsync(dto.TeamBId, cancellationToken: cancellationToken)
            ?? throw new KeyNotFoundException("L'équipe B demandée n'existe pas.");
        if (!await unitOfWork.TournamentRegistrations.ExistsAsync(dto.TournamentId, dto.TeamAId, cancellationToken)
            || !await unitOfWork.TournamentRegistrations.ExistsAsync(dto.TournamentId, dto.TeamBId, cancellationToken))
            throw new InvalidOperationException("Les deux équipes doivent être inscrites au tournoi.");

        var match = new Match
        {
            TournamentId = dto.TournamentId,
            TeamAId = dto.TeamAId,
            TeamBId = dto.TeamBId,
            ScheduledAt = dto.ScheduledAt
        };
        await unitOfWork.Matches.AddAsync(match, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return (await GetByIdAsync(match.Id, cancellationToken))!;
    }

    public async Task<bool> UpdateScoreAsync(int id, UpdateMatchScoreDto dto, CancellationToken cancellationToken)
    {
        var match = await unitOfWork.Matches.GetByIdAsync(id, cancellationToken: cancellationToken);
        if (match is null)
            return false;
        if (dto.ScoreTeamA < 0 || dto.ScoreTeamB < 0)
            throw new ArgumentException("Les scores ne peuvent pas être négatifs.");
        match.ScoreTeamA = dto.ScoreTeamA;
        match.ScoreTeamB = dto.ScoreTeamB;
        match.PlayedAt = dto.PlayedAt ?? DateTime.UtcNow;
        match.WinnerTeamId = dto.ScoreTeamA == dto.ScoreTeamB
            ? null
            : dto.ScoreTeamA > dto.ScoreTeamB ? match.TeamAId : match.TeamBId;
        unitOfWork.Matches.Update(match);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var match = await unitOfWork.Matches.GetByIdAsync(id, cancellationToken: cancellationToken);
        if (match is null)
            return false;
        unitOfWork.Matches.Delete(match);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static MatchResponseDto ToResponse(Match match) =>
        new(match.Id, match.TournamentId, match.TeamAId, match.TeamA?.Name ?? string.Empty,
            match.TeamBId, match.TeamB?.Name ?? string.Empty, match.WinnerTeamId,
            match.ScoreTeamA, match.ScoreTeamB, match.ScheduledAt, match.PlayedAt);
    private static int GetTotalPages(int totalCount, int pageSize) => (int)Math.Ceiling(totalCount / (double)pageSize);
}
