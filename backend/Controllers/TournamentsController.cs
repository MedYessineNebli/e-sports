using API.DTOs;
using API.CQRS.Tournaments;
using API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;

namespace API.Controllers;

[ApiController]
[Authorize]
[Route("api/tournaments")]
public sealed class TournamentsController(
    ITournamentService service,
    ITournamentRegistrationService registrationService,
    IMatchService matchService,
    ISender sender) : ControllerBase
{
    [HttpGet]
    public Task<PagedResult<TournamentResponseDto>> GetAll(
        [FromQuery] PagedRequest request,
        [FromQuery] int? gameId,
        [FromQuery] string? search,
        CancellationToken cancellationToken) =>
        service.GetPagedAsync(request, gameId, search, cancellationToken);

    [HttpGet("{id:int}")]
    public async Task<ActionResult<TournamentResponseDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var result = await service.GetByIdAsync(id, cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpGet("{id:int}/registrations")]
    [Authorize(Roles = "Admin,Organizer")]
    public Task<PagedResult<RegistrationResponseDto>> GetRegistrations(
        int id,
        [FromQuery] PagedRequest request,
        CancellationToken cancellationToken) =>
        registrationService.GetPagedAsync(request, id, null, cancellationToken);

    [HttpGet("{id:int}/matches")]
    [Authorize(Roles = "Admin,Organizer")]
    public Task<PagedResult<MatchResponseDto>> GetMatches(
        int id,
        [FromQuery] PagedRequest request,
        CancellationToken cancellationToken) =>
        matchService.GetPagedAsync(request, id, cancellationToken);

    [HttpGet("{id:int}/standings")]
    public async Task<ActionResult<IReadOnlyList<StandingResponseDto>>> GetStandings(
        int id,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetTournamentStandingsQuery(id), cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Organizer")]
    public async Task<ActionResult<TournamentResponseDto>> Create(CreateTournamentDto dto, CancellationToken cancellationToken)
    {
        try
        {
            var result = await service.CreateAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (KeyNotFoundException exception)
        {
            return NotFound(new ProblemDetails { Title = "Not Found", Detail = exception.Message, Status = 404 });
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new ProblemDetails { Title = "Bad Request", Detail = exception.Message, Status = 400 });
        }
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin,Organizer")]
    public async Task<IActionResult> Update(int id, UpdateTournamentDto dto, CancellationToken cancellationToken)
    {
        try
        {
            return await service.UpdateAsync(id, dto, cancellationToken) ? NoContent() : NotFound();
        }
        catch (KeyNotFoundException exception)
        {
            return NotFound(new ProblemDetails { Title = "Not Found", Detail = exception.Message, Status = 404 });
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new ProblemDetails { Title = "Bad Request", Detail = exception.Message, Status = 400 });
        }
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken) =>
        await service.DeleteAsync(id, cancellationToken) ? NoContent() : NotFound();
}
