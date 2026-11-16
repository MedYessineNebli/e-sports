using API.DTOs;
using API.Repositories;

namespace API.Services;

public sealed class TournamentService(IUnitOfWork unitOfWork) : ITournamentService
{
    public async Task<PagedResult<TournamentResponseDto>> GetPagedAsync(PagedRequest request, int? gameId, string? search, CancellationToken cancellationToken)
    {
        var page = request.ValidatedPage;
        var pageSize = request.ValidatedPageSize;
        var totalCount = await unitOfWork.Tournaments.CountAsync(gameId, search, cancellationToken);
        var items = await unitOfWork.Tournaments.GetPagedAsync(page, pageSize, gameId, search, cancellationToken);
        return new(page, pageSize, totalCount, GetTotalPages(totalCount, pageSize), items.Select(ToResponse).ToList());
    }

    public async Task<TournamentResponseDto?> GetByIdAsync(int id, CancellationToken cancellationToken) =>
        (await unitOfWork.Tournaments.GetByIdAsync(id, cancellationToken: cancellationToken)) is { } tournament
            ? ToResponse(tournament)
            : null;

    public async Task<IReadOnlyList<StandingResponseDto>?> GetStandingsAsync(int id, CancellationToken cancellationToken)
    {
        var tournament = await unitOfWork.Tournaments.GetByIdAsync(id, true, cancellationToken: cancellationToken);
        if (tournament is null)
            return null;

        var standings = tournament.TournamentRegistrations
            .Select(registration => new StandingAccumulator(registration.TeamId, registration.Team.Name))
            .ToDictionary(standing => standing.TeamId);

        var matches = await unitOfWork.Matches.GetCompletedByTournamentAsync(id, cancellationToken);
        foreach (var match in matches)
        {
            if (!standings.TryGetValue(match.TeamAId, out var teamA)
                || !standings.TryGetValue(match.TeamBId, out var teamB))
                continue;

            teamA.Played++;
            teamB.Played++;
            if (match.ScoreTeamA == match.ScoreTeamB)
            {
                teamA.Draws++;
                teamB.Draws++;
                teamA.Points++;
                teamB.Points++;
            }
            else if (match.ScoreTeamA > match.ScoreTeamB)
            {
                teamA.Wins++;
                teamB.Losses++;
                teamA.Points += 3;
            }
            else
            {
                teamB.Wins++;
                teamA.Losses++;
                teamB.Points += 3;
            }
        }

        return standings.Values
            .OrderByDescending(standing => standing.Points)
            .ThenByDescending(standing => standing.Wins)
            .ThenBy(standing => standing.TeamName)
            .Select(standing => new StandingResponseDto(
                standing.TeamId, standing.TeamName, standing.Played,
                standing.Wins, standing.Losses, standing.Draws, standing.Points))
            .ToList();
    }

    public async Task<TournamentResponseDto> CreateAsync(CreateTournamentDto dto, CancellationToken cancellationToken)
    {
        ValidateDates(dto.StartDate, dto.EndDate);
        var game = await unitOfWork.Games.GetByIdAsync(dto.GameId, cancellationToken: cancellationToken)
            ?? throw new KeyNotFoundException("Le jeu demandé n'existe pas.");
        var tournament = new Tournament
        {
            Name = dto.Name.Trim(),
            GameId = game.Id,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate
        };
        await unitOfWork.Tournaments.AddAsync(tournament, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return ToResponse(tournament);
    }

    public async Task<bool> UpdateAsync(int id, UpdateTournamentDto dto, CancellationToken cancellationToken)
    {
        ValidateDates(dto.StartDate, dto.EndDate);
        var tournament = await unitOfWork.Tournaments.GetByIdAsync(id, cancellationToken: cancellationToken);
        if (tournament is null)
            return false;
        if (await unitOfWork.Games.GetByIdAsync(dto.GameId, cancellationToken: cancellationToken) is null)
            throw new KeyNotFoundException("Le jeu demandé n'existe pas.");
        tournament.Name = dto.Name.Trim();
        tournament.GameId = dto.GameId;
        tournament.StartDate = dto.StartDate;
        tournament.EndDate = dto.EndDate;
        unitOfWork.Tournaments.Update(tournament);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var tournament = await unitOfWork.Tournaments.GetByIdAsync(id, true, true, cancellationToken);
        if (tournament is null)
            return false;
        if (tournament.TournamentRegistrations.Count > 0 || tournament.Matches.Count > 0)
            throw new InvalidOperationException("Ce tournoi ne peut pas être supprimé car il possède des inscriptions ou des matchs.");
        unitOfWork.Tournaments.Delete(tournament);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static void ValidateDates(DateTime startDate, DateTime endDate)
    {
        if (startDate >= endDate)
            throw new ArgumentException("La date de début doit être antérieure à la date de fin.");
    }

    private static TournamentResponseDto ToResponse(Tournament tournament) =>
        new(tournament.Id, tournament.Name, tournament.GameId, tournament.StartDate, tournament.EndDate);
    private static int GetTotalPages(int totalCount, int pageSize) => (int)Math.Ceiling(totalCount / (double)pageSize);

    private sealed class StandingAccumulator(int teamId, string teamName)
    {
        public int TeamId { get; } = teamId;
        public string TeamName { get; } = teamName;
        public int Played { get; set; }
        public int Wins { get; set; }
        public int Losses { get; set; }
        public int Draws { get; set; }
        public int Points { get; set; }
    }
}
