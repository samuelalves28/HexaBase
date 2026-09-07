namespace HexaBase.Domain.Shared.Bases;

public interface IBaseRepository<TModel>
{
    Task<TModel?> GetByIdAsync(int id, CancellationToken cancellation);
    Task<TModel?> GetByPublicIdAsync(Guid publicId, CancellationToken cancellation);
    Task<List<TModel>> GetByPublicIdsAsync(List<Guid> publicIds, CancellationToken cancellation);
    Task CreateAsync(TModel model, CancellationToken cancellation);
    Task CreateAsync(List<TModel> model, CancellationToken cancellation);
    Task UpdateAsync(List<TModel> model, CancellationToken cancellation);
    Task UpdateAsync(TModel model, CancellationToken cancellation);
    Task DeleteAsync(int id, CancellationToken cancellation);
}
