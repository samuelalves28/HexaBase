namespace HexaBase.Api.Adapters.In.Http.DTOs;

public sealed record GetUserResponse(
    Guid PublicId,
    string Name,
    string Email,
    DateTime CreatedAt,
    DateTime? UpdatedAt);