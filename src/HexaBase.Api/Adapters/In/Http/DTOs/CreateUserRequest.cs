using System.ComponentModel.DataAnnotations;

namespace HexaBase.Api.Adapters.In.Http.DTOs;

public sealed record CreateUserRequest(
    [property: Required] string Name,
    [property: Required, EmailAddress] string Email);