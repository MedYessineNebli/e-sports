using API.DTOs;

namespace API.Services;

public interface ITournamentService
{
    Task<PagedResult<TournamentResponseDto>> GetPagedAsync(PagedRequest request, int? gameId, string? search, CancellationToken cancellationToken);
    Task<TournamentResponseDto?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<IReadOnlyList<StandingResponseDto>?> GetStandingsAsync(int id, CancellationToken cancellationToken);
    Task<TournamentResponseDto> CreateAsync(CreateTournamentDto dto, CancellationToken cancellationToken);
    Task<bool> UpdateAsync(int id, UpdateTournamentDto dto, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken);
}
