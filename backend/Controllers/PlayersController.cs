using API.DTOs;
using API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Authorize(Roles = "Admin,Organizer")]
[Route("api/players")]
public sealed class PlayersController(IPlayerService service) : ControllerBase
{
    [HttpGet]
    public Task<PagedResult<PlayerResponseDto>> GetAll(
        [FromQuery] PagedRequest request,
        [FromQuery] int? teamId,
        [FromQuery] string? search,
        CancellationToken cancellationToken) =>
        service.GetPagedAsync(request, teamId, search, cancellationToken);

    [HttpGet("{id:int}")]
    public async Task<ActionResult<PlayerResponseDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var result = await service.GetByIdAsync(id, cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Organizer")]
    public async Task<ActionResult<PlayerResponseDto>> Create(CreatePlayerDto dto, CancellationToken cancellationToken)
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
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin,Organizer")]
    public async Task<IActionResult> Update(int id, UpdatePlayerDto dto, CancellationToken cancellationToken)
    {
        try
        {
            return await service.UpdateAsync(id, dto, cancellationToken) ? NoContent() : NotFound();
        }
        catch (KeyNotFoundException exception)
        {
            return NotFound(new ProblemDetails { Title = "Not Found", Detail = exception.Message, Status = 404 });
        }
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken) =>
        await service.DeleteAsync(id, cancellationToken) ? NoContent() : NotFound();
}
