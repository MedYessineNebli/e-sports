using API.DTOs;

namespace API.Services;

public interface IMatchService
{
    Task<PagedResult<MatchResponseDto>> GetPagedAsync(PagedRequest request, int? tournamentId, CancellationToken cancellationToken);
    Task<MatchResponseDto?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<MatchResponseDto> CreateAsync(CreateMatchDto dto, CancellationToken cancellationToken);
    Task<bool> UpdateScoreAsync(int id, UpdateMatchScoreDto dto, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken);
}
