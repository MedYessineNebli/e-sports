using API.DTOs;
using API.CQRS.Games;
using API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;

namespace API.Controllers;

[ApiController]
[Authorize]
[Route("api/games")]
public sealed class GamesController(IGameService service, ISender sender) : ControllerBase
{
    [HttpGet]
    public Task<PagedResult<GameResponseDto>> GetAll(
        [FromQuery] PagedRequest request,
        [FromQuery] string? search,
        CancellationToken cancellationToken) =>
        sender.Send(new GetGamesQuery(request, search), cancellationToken);

    [HttpGet("{id:int}")]
    public async Task<ActionResult<GameResponseDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetGameByIdQuery(id), cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Organizer")]
    public async Task<ActionResult<GameResponseDto>> Create(CreateGameDto dto, CancellationToken cancellationToken)
    {
        try
        {
            var result = await sender.Send(new CreateGameCommand(dto), cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (InvalidOperationException exception)
        {
            return Conflict(new ProblemDetails { Title = "Conflict", Detail = exception.Message, Status = 409 });
        }
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin,Organizer")]
    public async Task<IActionResult> Update(int id, UpdateGameDto dto, CancellationToken cancellationToken) =>
        await service.UpdateAsync(id, dto, cancellationToken) ? NoContent() : NotFound();

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken) =>
        await sender.Send(new DeleteGameCommand(id), cancellationToken) ? NoContent() : NotFound();
}
