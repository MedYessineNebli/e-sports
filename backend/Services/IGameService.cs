using API.DTOs;

namespace API.Services;

public interface IGameService
{
    Task<PagedResult<GameResponseDto>> GetPagedAsync(PagedRequest request, string? search, CancellationToken cancellationToken);
    Task<GameResponseDto?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<GameResponseDto> CreateAsync(CreateGameDto dto, CancellationToken cancellationToken);
    Task<bool> UpdateAsync(int id, UpdateGameDto dto, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken);
}
