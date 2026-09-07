using HexaBase.Domain.Shared.Bases;
using Microsoft.EntityFrameworkCore;

namespace HexaBase.Infrastructure.Adapters.Out.Persistence.Base
{
    public class BaseRepository<TModel> : IBaseRepository<TModel> where TModel : BaseEntity
    {
        protected readonly HexaBaseDbContext _context;
        protected readonly DbSet<TModel> _dbSet;

        public BaseRepository(HexaBaseDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<TModel>();
        }

        public async Task<TModel?> GetByIdAsync(int id, CancellationToken cancellation) => await _dbSet.SingleOrDefaultAsync(x => x.Id == id, cancellation);

        public async Task<TModel?> GetByPublicIdAsync(Guid publicId, CancellationToken cancellation)
            => await _dbSet.SingleOrDefaultAsync(x => x.PublicId == publicId, cancellation);

        public async Task<List<TModel>> GetByPublicIdsAsync(List<Guid> publicIds, CancellationToken cancellation)
            => await _dbSet.Where(w => publicIds.Contains(w.PublicId)).ToListAsync(cancellation);

        public async Task CreateAsync(TModel model, CancellationToken cancellation)
        {
            await _dbSet.AddAsync(model, cancellation);
            await SaveChangesAsync(cancellation);
        }

        public async Task CreateAsync(List<TModel> model, CancellationToken cancellation)
        {
            await _dbSet.AddRangeAsync(model, cancellation);
            await SaveChangesAsync(cancellation);
        }

        public async Task UpdateAsync(TModel model, CancellationToken cancellation)
        {
            _dbSet.Update(model);
            await SaveChangesAsync(cancellation);
        }
        public async Task UpdateAsync(List<TModel> model, CancellationToken cancellation)
        {
            _dbSet.UpdateRange(model);
            await SaveChangesAsync(cancellation);
        }

        public async Task DeleteAsync(int id, CancellationToken cancellation)
        {
            await _dbSet.Where(x => x.Id == id).ExecuteDeleteAsync(cancellation);
            await SaveChangesAsync(cancellation);
        }

        protected async Task SaveChangesAsync(CancellationToken cancellationToken) => await _context.SaveChangesAsync(cancellationToken: cancellationToken);
    }
}

