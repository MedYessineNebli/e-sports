using API.DTOs;
using MediatR;

namespace API.CQRS.Games;

public sealed record CreateGameCommand(CreateGameDto Dto) : IRequest<GameResponseDto>;

public sealed record DeleteGameCommand(int Id) : IRequest<bool>;
