using API.DTOs;
using MediatR;

namespace API.CQRS.Games;

public sealed record GetGamesQuery(
    PagedRequest Request,
    string? Search) : IRequest<PagedResult<GameResponseDto>>;

public sealed record GetGameByIdQuery(int Id) : IRequest<GameResponseDto?>;
