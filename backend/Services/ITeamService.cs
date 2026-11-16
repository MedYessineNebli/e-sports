using API.DTOs;

namespace API.Services;

public interface ITeamService
{
    Task<PagedResult<TeamResponseDto>> GetPagedAsync(PagedRequest request, string? search, CancellationToken cancellationToken);
    Task<TeamResponseDto?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<TeamResponseDto> CreateAsync(CreateTeamDto dto, CancellationToken cancellationToken);
    Task<bool> UpdateAsync(int id, UpdateTeamDto dto, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken);
}
