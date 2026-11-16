using API.DTOs;
using API.Repositories;

namespace API.Services;

public sealed class GameService(IUnitOfWork unitOfWork) : IGameService
{
    public async Task<PagedResult<GameResponseDto>> GetPagedAsync(PagedRequest request, string? search, CancellationToken cancellationToken)
    {
        var page = request.ValidatedPage;
        var pageSize = request.ValidatedPageSize;
        var totalCount = await unitOfWork.Games.CountAsync(search, cancellationToken);
        var items = await unitOfWork.Games.GetPagedAsync(page, pageSize, search, cancellationToken);
        return new(page, pageSize, totalCount, GetTotalPages(totalCount, pageSize), items.Select(ToResponse).ToList());
    }

    public async Task<GameResponseDto?> GetByIdAsync(int id, CancellationToken cancellationToken) =>
        (await unitOfWork.Games.GetByIdAsync(id, cancellationToken: cancellationToken)) is { } game
            ? ToResponse(game)
            : null;

    public async Task<GameResponseDto> CreateAsync(CreateGameDto dto, CancellationToken cancellationToken)
    {
        if (await unitOfWork.Games.CountAsync(dto.Name, cancellationToken) > 0)
            throw new InvalidOperationException("Un jeu portant ce nom existe déjà.");
        var game = new Game { Name = dto.Name.Trim(), Description = dto.Description?.Trim() };
        await unitOfWork.Games.AddAsync(game, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return ToResponse(game);
    }

    public async Task<bool> UpdateAsync(int id, UpdateGameDto dto, CancellationToken cancellationToken)
    {
        var game = await unitOfWork.Games.GetByIdAsync(id, cancellationToken: cancellationToken);
        if (game is null)
            return false;
        game.Name = dto.Name.Trim();
        game.Description = dto.Description?.Trim();
        unitOfWork.Games.Update(game);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var game = await unitOfWork.Games.GetByIdAsync(id, cancellationToken: cancellationToken);
        if (game is null)
            return false;
        unitOfWork.Games.Delete(game);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static GameResponseDto ToResponse(Game game) => new(game.Id, game.Name, game.Description);
    private static int GetTotalPages(int totalCount, int pageSize) => (int)Math.Ceiling(totalCount / (double)pageSize);
}
