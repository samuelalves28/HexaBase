namespace HexaBase.Application.Aggregates.Users.Queries.GetUserByPublicIdQuery;

public sealed record GetUserByPublicIdResponse(
    Guid PublicId,
    string Name,
    string Email,
    DateTime CreatedAt,
    DateTime? UpdatedAt);