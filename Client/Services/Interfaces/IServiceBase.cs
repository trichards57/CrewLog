using BlazorApp.Shared.Interfaces;

namespace BlazorApp.Client.Services.Interfaces
{
    public interface IServiceBase<TCreate, TItem>
        where TCreate : IIdentifiable
    {
        Task<TItem?> CreateAsync(TCreate item, CancellationToken? cancellationToken = null);
        Task<TItem?> UpdateAsync(TItem item, CancellationToken? cancellationToken = null);
        Task<bool> DeleteAsync(string id, CancellationToken? cancellationToken = null);
        Task<TItem?> GetItemAsync(string id, CancellationToken? cancellationToken= null);
        IAsyncEnumerable<TItem> GetAllAsync(string id, CancellationToken? cancellationToken= null);
    }
}
