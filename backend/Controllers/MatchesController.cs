using API.DTOs;
using API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Authorize(Roles = "Admin,Organizer")]
[Route("api/matches")]
public sealed class MatchesController(IMatchService service) : ControllerBase
{
    [HttpGet]
    public Task<PagedResult<MatchResponseDto>> GetAll(
        [FromQuery] PagedRequest request,
        [FromQuery] int? tournamentId,
        CancellationToken cancellationToken) =>
        service.GetPagedAsync(request, tournamentId, cancellationToken);

    [HttpGet("results")]
    [Authorize(Roles = "Admin,Organizer,Player")]
    public Task<PagedResult<MatchResponseDto>> GetResults(
        [FromQuery] PagedRequest request,
        [FromQuery] int? tournamentId,
        CancellationToken cancellationToken) =>
        service.GetCompletedPagedAsync(request, tournamentId, cancellationToken);

    [HttpGet("{id:int}")]
    public async Task<ActionResult<MatchResponseDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var result = await service.GetByIdAsync(id, cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Organizer")]
    public async Task<ActionResult<MatchResponseDto>> Create(CreateMatchDto dto, CancellationToken cancellationToken)
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
        catch (ArgumentException exception)
        {
            return BadRequest(new ProblemDetails { Title = "Bad Request", Detail = exception.Message, Status = 400 });
        }
    }

    [HttpPatch("{id:int}/score")]
    [Authorize(Roles = "Admin,Organizer")]
    public async Task<IActionResult> UpdateScore(int id, UpdateMatchScoreDto dto, CancellationToken cancellationToken)
    {
        try
        {
            return await service.UpdateScoreAsync(id, dto, cancellationToken) ? NoContent() : NotFound();
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
