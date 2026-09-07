using HexaBase.Domain.Aggregates.User;
using HexaBase.Domain.Aggregates.User.Repository;
using HexaBase.Infrastructure.Adapters.Out.Persistence.Base;

namespace HexaBase.Infrastructure.Adapters.Out.Persistence.Repositories;

public sealed class UserRepository(HexaBaseDbContext context) : BaseRepository<User>(context), IUserRepository
{

}