using HexaBase.Domain.Aggregates.User;
using HexaBase.Domain.Aggregates.User.Repository;
using HexaBase.Infrastructure.Adapters.Out.Persistence;
using HexaBase.Infrastructure.Adapters.Out.Persistence.Base;

namespace HexaBase.Infrastructure.Adapters.Persistence.Repositories;

public sealed class UserRepository(HexaBaseDbContext context) : BaseRepository<User>(context), IUserRepository
{

}