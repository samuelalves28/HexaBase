using HexaBase.Domain.Aggregates.User;
using HexaBase.Domain.Aggregates.User.Repository;
using HexaBase.Domain.Shared.Bases;
using HexaBase.Infrastructure.Adapters.Out.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HexaBase.Infrastructure.Adapters.Out.Persistence.Repositories;

public sealed class UserRepository : IUserRepository
{
    private readonly HexaBaseDbContext _context;

    public UserRepository(HexaBaseDbContext context)
    {
        _context = context;
    }

    public async Task CreateAsync(
        User model,
        CancellationToken cancellation)
    {
        await _context.Users.AddAsync(model, cancellation);
        await _context.SaveChangesAsync(cancellation);
    }

    public async Task CreateAsync(
        List<User> model,
        CancellationToken cancellation)
    {
        await _context.Users.AddRangeAsync(model, cancellation);
        await _context.SaveChangesAsync(cancellation);
    }

    public async Task DeleteAsync(
        int id,
        CancellationToken cancellation)
    {
        var entity = await _context.Users.FirstOrDefaultAsync(x => x.Id == id, cancellation);
        if (entity is null)
        {
            return;
        }

        _context.Users.Remove(entity);
        await _context.SaveChangesAsync(cancellation);
    }

    public async Task<User?> GetByIdAsync(
        int id,
        CancellationToken cancellation)
    {
        return await _context.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellation);
    }

    public async Task<User?> GetByPublicIdAsync(
        Guid publicId,
        CancellationToken cancellation)
    {
        return await _context.Users.AsNoTracking().FirstOrDefaultAsync(x => x.PublicId == publicId, cancellation);
    }

    public async Task<List<User>> GetByPublicIdsAsync(
        List<Guid> publicIds,
        CancellationToken cancellation)
    {
        return await _context.Users.AsNoTracking().Where(x => publicIds.Contains(x.PublicId)).ToListAsync(cancellation);
    }

    public async Task UpdateAsync(
        List<User> model,
        CancellationToken cancellation)
    {
        foreach (var item in model)
        {
            item.ChangeLastUpdate();
        }

        _context.Users.UpdateRange(model);
        await _context.SaveChangesAsync(cancellation);
    }

    public async Task UpdateAsync(
        User model,
        CancellationToken cancellation)
    {
        model.ChangeLastUpdate();
        _context.Users.Update(model);
        await _context.SaveChangesAsync(cancellation);
    }
}