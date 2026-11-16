using API.DTOs;

namespace API.Services;

public interface ITournamentRegistrationService
{
    Task<PagedResult<RegistrationResponseDto>> GetPagedAsync(PagedRequest request, int? tournamentId, int? teamId, CancellationToken cancellationToken);
    Task<RegistrationResponseDto?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<RegistrationResponseDto> CreateAsync(CreateRegistrationDto dto, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken);
}
