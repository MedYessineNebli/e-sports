using API.DTOs;
using API.Services;
using MediatR;

namespace API.CQRS.Games;

public sealed class GetGamesHandler(IGameService service)
    : IRequestHandler<GetGamesQuery, PagedResult<GameResponseDto>>
{
    public Task<PagedResult<GameResponseDto>> Handle(GetGamesQuery request, CancellationToken cancellationToken) =>
        service.GetPagedAsync(request.Request, request.Search, cancellationToken);
}

public sealed class GetGameByIdHandler(IGameService service)
    : IRequestHandler<GetGameByIdQuery, GameResponseDto?>
{
    public Task<GameResponseDto?> Handle(GetGameByIdQuery request, CancellationToken cancellationToken) =>
        service.GetByIdAsync(request.Id, cancellationToken);
}

public sealed class CreateGameHandler(IGameService service)
    : IRequestHandler<CreateGameCommand, GameResponseDto>
{
    public Task<GameResponseDto> Handle(CreateGameCommand request, CancellationToken cancellationToken) =>
        service.CreateAsync(request.Dto, cancellationToken);
}

public sealed class DeleteGameHandler(IGameService service)
    : IRequestHandler<DeleteGameCommand, bool>
{
    public Task<bool> Handle(DeleteGameCommand request, CancellationToken cancellationToken) =>
        service.DeleteAsync(request.Id, cancellationToken);
}
