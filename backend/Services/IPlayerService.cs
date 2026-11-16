using API.DTOs;

namespace API.Services;

public interface IPlayerService
{
    Task<PagedResult<PlayerResponseDto>> GetPagedAsync(PagedRequest request, int? teamId, string? search, CancellationToken cancellationToken);
    Task<PlayerResponseDto?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<PlayerResponseDto> CreateAsync(CreatePlayerDto dto, CancellationToken cancellationToken);
    Task<bool> UpdateAsync(int id, UpdatePlayerDto dto, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken);
}
