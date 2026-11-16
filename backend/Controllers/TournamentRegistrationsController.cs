using API.DTOs;
using API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Authorize]
[Route("api/tournament-registrations")]
public sealed class TournamentRegistrationsController(ITournamentRegistrationService service) : ControllerBase
{
    [HttpGet]
    public Task<PagedResult<RegistrationResponseDto>> GetAll(
        [FromQuery] PagedRequest request,
        [FromQuery] int? tournamentId,
        [FromQuery] int? teamId,
        CancellationToken cancellationToken) =>
        service.GetPagedAsync(request, tournamentId, teamId, cancellationToken);

    [HttpGet("{id:int}")]
    public async Task<ActionResult<RegistrationResponseDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var result = await service.GetByIdAsync(id, cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Organizer")]
    public async Task<ActionResult<RegistrationResponseDto>> Create(CreateRegistrationDto dto, CancellationToken cancellationToken)
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
        catch (InvalidOperationException exception)
        {
            return Conflict(new ProblemDetails { Title = "Conflict", Detail = exception.Message, Status = 409 });
        }
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin,Organizer")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken) =>
        await service.DeleteAsync(id, cancellationToken) ? NoContent() : NotFound();
}
