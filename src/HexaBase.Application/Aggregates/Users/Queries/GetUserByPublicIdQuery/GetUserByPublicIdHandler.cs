using HexaBase.Domain.Aggregates.User.Repository;
using MediatR;

namespace HexaBase.Application.Aggregates.Users.Queries.GetUserByPublicIdQuery;

public sealed class GetUserByPublicIdHandler : IRequestHandler<GetUserByPublicIdQuery, GetUserByPublicIdResponse?>
{
    private readonly IUserRepository _userRepository;

    public GetUserByPublicIdHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<GetUserByPublicIdResponse?> Handle(
        GetUserByPublicIdQuery request,
        CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByPublicIdAsync(
            request.PublicId,
            cancellationToken);

        if (user is null)
        {
            return null;
        }

        return new GetUserByPublicIdResponse(
            user.PublicId,
            user.Name,
            user.Email,
            user.CreatedAt,
            user.UpdatedAt);
    }
}