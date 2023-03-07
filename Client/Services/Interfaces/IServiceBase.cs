using CrewLog.Shared.Interfaces;

namespace CrewLog.Client.Services.Interfaces
{
    public interface IServiceBase<TItem>
        where TItem : IIdentifiable
    {
        Task<TItem?> CreateAsync(TItem item, CancellationToken? cancellationToken = null);
        Task<TItem?> UpdateAsync(TItem item, CancellationToken? cancellationToken = null);
        Task<bool> DeleteAsync(string id, CancellationToken? cancellationToken = null);
        Task<TItem?> GetItemAsync(string id, CancellationToken? cancellationToken= null);
        IAsyncEnumerable<TItem> GetAllAsync(CancellationToken? cancellationToken= null);
    }
}
