using API.DTOs;
using API.Repositories;

namespace API.Services;

public sealed class PlayerService(IUnitOfWork unitOfWork) : IPlayerService
{
    public async Task<PagedResult<PlayerResponseDto>> GetPagedAsync(PagedRequest request, int? teamId, string? search, CancellationToken cancellationToken)
    {
        var page = request.ValidatedPage;
        var pageSize = request.ValidatedPageSize;
        var totalCount = await unitOfWork.Players.CountAsync(teamId, search, cancellationToken);
        var items = await unitOfWork.Players.GetPagedAsync(page, pageSize, teamId, search, cancellationToken);
        return new(page, pageSize, totalCount, GetTotalPages(totalCount, pageSize),
            items.Select(player => new PlayerResponseDto(player.Id, player.Gamertag, player.TeamId, null)).ToList());
    }

    public async Task<PlayerResponseDto?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        var player = await unitOfWork.Players.GetByIdAsync(id, true, cancellationToken);
        return player is null ? null : ToResponse(player);
    }

    public async Task<PlayerResponseDto> CreateAsync(CreatePlayerDto dto, CancellationToken cancellationToken)
    {
        var team = await unitOfWork.Teams.GetByIdAsync(dto.TeamId, cancellationToken: cancellationToken)
            ?? throw new KeyNotFoundException("L'équipe demandée n'existe pas.");
        var player = new Player { Gamertag = dto.Gamertag.Trim(), TeamId = team.Id, Team = team };
        await unitOfWork.Players.AddAsync(player, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return new(player.Id, player.Gamertag, player.TeamId, team.Name);
    }

    public async Task<bool> UpdateAsync(int id, UpdatePlayerDto dto, CancellationToken cancellationToken)
    {
        var player = await unitOfWork.Players.GetByIdAsync(id, cancellationToken: cancellationToken);
        if (player is null)
            return false;
        var team = await unitOfWork.Teams.GetByIdAsync(dto.TeamId, cancellationToken: cancellationToken)
            ?? throw new KeyNotFoundException("L'équipe demandée n'existe pas.");
        player.Gamertag = dto.Gamertag.Trim();
        player.TeamId = team.Id;
        unitOfWork.Players.Update(player);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var player = await unitOfWork.Players.GetByIdAsync(id, cancellationToken: cancellationToken);
        if (player is null)
            return false;
        unitOfWork.Players.Delete(player);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static PlayerResponseDto ToResponse(Player player) =>
        new(player.Id, player.Gamertag, player.TeamId, player.Team?.Name);
    private static int GetTotalPages(int totalCount, int pageSize) => (int)Math.Ceiling(totalCount / (double)pageSize);
}
