using BlazorApp.Client.Services.Interfaces;
using BlazorApp.Shared.Interfaces;

namespace BlazorApp.Client.Services
{
    public class ServiceBase<TItem> : IServiceBase<TItem>
        where TItem : IIdentifiable
    {
        public Task<TItem?> CreateAsync(TItem item, CancellationToken? cancellationToken = null)
        {
            return Task.FromResult<TItem?>(default);
        }

        public Task<bool> DeleteAsync(string id, CancellationToken? cancellationToken = null)
        {
            return Task.FromResult(false);
        }

        public IAsyncEnumerable<TItem> GetAllAsync(CancellationToken? cancellationToken = null)
        {
            return AsyncEnumerable.Empty<TItem>();
        }

        public Task<TItem?> GetItemAsync(string id, CancellationToken? cancellationToken = null)
        {
            return Task.FromResult<TItem?>(default);
        }

        public Task<TItem?> UpdateAsync(TItem item, CancellationToken? cancellationToken = null)
        {
            return Task.FromResult<TItem?>(default);
        }
    }
}
