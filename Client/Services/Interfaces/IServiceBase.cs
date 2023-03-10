using CrewLog.Shared.Interfaces;

namespace CrewLog.Client.Services.Interfaces
{
    public interface IServiceBase<TItem>
        where TItem : IIdentifiable
    {
        Task<TItem?> CreateAsync(TItem item, CancellationToken cancellationToken = default);
        Task<TItem?> UpdateAsync(TItem item, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
        Task<TItem?> GetItemAsync(Guid id, CancellationToken cancellationToken = default);
        IAsyncEnumerable<TItem> GetAllAsync(CancellationToken cancellationToken = default);
    }
}
