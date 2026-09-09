namespace HexaBase.Api.Adapters.Http.DTOs;

public sealed record GetUserResponse(
    Guid PublicId,
    string Name,
    string Email,
    DateTime CreatedAt,
    DateTime? UpdatedAt);