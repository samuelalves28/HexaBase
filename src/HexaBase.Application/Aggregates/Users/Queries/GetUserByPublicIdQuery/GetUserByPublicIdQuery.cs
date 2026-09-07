using MediatR;

namespace HexaBase.Application.Aggregates.Users.Queries.GetUserByPublicIdQuery;

public sealed record GetUserByPublicIdQuery(Guid PublicId) : IRequest<GetUserByPublicIdResponse?>;