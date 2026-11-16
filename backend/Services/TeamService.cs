using API.DTOs;
using API.Repositories;

namespace API.Services;

public sealed class TeamService(IUnitOfWork unitOfWork) : ITeamService
{
    public async Task<PagedResult<TeamResponseDto>> GetPagedAsync(PagedRequest request, string? search, CancellationToken cancellationToken)
    {
        var page = request.ValidatedPage;
        var pageSize = request.ValidatedPageSize;
        var totalCount = await unitOfWork.Teams.CountAsync(search, cancellationToken);
        var items = await unitOfWork.Teams.GetPagedAsync(page, pageSize, search, cancellationToken);
        return new(page, pageSize, totalCount, GetTotalPages(totalCount, pageSize), items.Select(ToResponse).ToList());
    }

    public async Task<TeamResponseDto?> GetByIdAsync(int id, CancellationToken cancellationToken) =>
        (await unitOfWork.Teams.GetByIdAsync(id, cancellationToken: cancellationToken)) is { } team
            ? ToResponse(team)
            : null;

    public async Task<TeamResponseDto> CreateAsync(CreateTeamDto dto, CancellationToken cancellationToken)
    {
        if (await unitOfWork.Teams.CountAsync(dto.Name, cancellationToken) > 0)
            throw new InvalidOperationException("Une équipe portant ce nom existe déjà.");
        var team = new Team { Name = dto.Name.Trim() };
        await unitOfWork.Teams.AddAsync(team, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return ToResponse(team);
    }

    public async Task<bool> UpdateAsync(int id, UpdateTeamDto dto, CancellationToken cancellationToken)
    {
        var team = await unitOfWork.Teams.GetByIdAsync(id, cancellationToken: cancellationToken);
        if (team is null)
            return false;
        team.Name = dto.Name.Trim();
        unitOfWork.Teams.Update(team);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var team = await unitOfWork.Teams.GetByIdAsync(id, cancellationToken: cancellationToken);
        if (team is null)
            return false;
        unitOfWork.Teams.Delete(team);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static TeamResponseDto ToResponse(Team team) => new(team.Id, team.Name);
    private static int GetTotalPages(int totalCount, int pageSize) => (int)Math.Ceiling(totalCount / (double)pageSize);
}
