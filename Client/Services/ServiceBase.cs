using CrewLog.Client.Services.Interfaces;
using CrewLog.Shared.Interfaces;

namespace CrewLog.Client.Services
{
    public class ServiceBase<TItem> : IServiceBase<TItem>
        where TItem : IIdentifiable
    {
        public Task<TItem?> CreateAsync(TItem item, CancellationToken? cancellationToken = null)
        {
            return Task.FromResult<TItem?>(default);
        }

        public Task<bool> DeleteAsync(Guid id, CancellationToken? cancellationToken = null)
        {
            return Task.FromResult(false);
        }

        public IAsyncEnumerable<TItem> GetAllAsync(CancellationToken? cancellationToken = null)
        {
            return AsyncEnumerable.Empty<TItem>();
        }

        public Task<TItem?> GetItemAsync(Guid id, CancellationToken? cancellationToken = null)
        {
            return Task.FromResult<TItem?>(default);
        }

        public Task<TItem?> UpdateAsync(TItem item, CancellationToken? cancellationToken = null)
        {
            return Task.FromResult<TItem?>(default);
        }
    }
}
