using API.DTOs;
using API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Authorize]
[Route("api/teams")]
public sealed class TeamsController(ITeamService service, IPlayerService playerService) : ControllerBase
{
    [HttpGet]
    public Task<PagedResult<TeamResponseDto>> GetAll(
        [FromQuery] PagedRequest request,
        [FromQuery] string? search,
        CancellationToken cancellationToken) =>
        service.GetPagedAsync(request, search, cancellationToken);

    [HttpGet("{id:int}")]
    public async Task<ActionResult<TeamResponseDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var result = await service.GetByIdAsync(id, cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpGet("{id:int}/players")]
    public Task<PagedResult<PlayerResponseDto>> GetPlayers(
        int id,
        [FromQuery] PagedRequest request,
        CancellationToken cancellationToken) =>
        playerService.GetPagedAsync(request, id, null, cancellationToken);

    [HttpPost]
    [Authorize(Roles = "Admin,Organizer")]
    public async Task<ActionResult<TeamResponseDto>> Create(CreateTeamDto dto, CancellationToken cancellationToken)
    {
        try
        {
            var result = await service.CreateAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (InvalidOperationException exception)
        {
            return Conflict(new ProblemDetails { Title = "Conflict", Detail = exception.Message, Status = 409 });
        }
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin,Organizer")]
    public async Task<IActionResult> Update(int id, UpdateTeamDto dto, CancellationToken cancellationToken) =>
        await service.UpdateAsync(id, dto, cancellationToken) ? NoContent() : NotFound();

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken) =>
        await service.DeleteAsync(id, cancellationToken) ? NoContent() : NotFound();
}
