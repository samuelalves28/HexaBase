using HexaBase.Api.Adapters.In.Http.DTOs;
using HexaBase.Application.Aggregates.Users.Commands.CreateUserCommand;
using HexaBase.Application.Aggregates.Users.Queries.GetUserByPublicIdQuery;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HexaBase.Api.Adapters.In.Http.Controllers;

[ApiController]
[Route("api/users")]
public sealed class UsersController : ControllerBase
{
    private readonly ISender _sender;

    public UsersController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateUserRequest request,
        CancellationToken cancellationToken)
    {
        var publicId = await _sender.Send(
            new CreateUserCommand(
                request.Name,
                request.Email),
            cancellationToken);

        return CreatedAtAction(
            nameof(GetByPublicId),
            new { publicId },
            new { publicId });
    }

    [HttpGet("{publicId:guid}")]
    public async Task<ActionResult<GetUserResponse>> GetByPublicId(
        [FromRoute] Guid publicId,
        CancellationToken cancellationToken)
    {
        var user = await _sender.Send(
            new GetUserByPublicIdQuery(publicId),
            cancellationToken);

        if (user is null)
        {
            return NotFound();
        }

        return Ok(new GetUserResponse(
            user.PublicId,
            user.Name,
            user.Email,
            user.CreatedAt,
            user.UpdatedAt));
    }
}