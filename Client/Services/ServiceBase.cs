using CommunityToolkit.Mvvm.Messaging;
using CrewLog.Client.Messages;
using CrewLog.Client.Services.Interfaces;
using CrewLog.Shared.Interfaces;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;

namespace CrewLog.Client.Services
{
    public abstract class ServiceBase<TItem> : IServiceBase<TItem>
        where TItem : IIdentifiable
    {
        private readonly HttpClient _client;
        private readonly string _endpoint;

        protected ServiceBase(HttpClient client, string endpoint)
        {
            _client = client;
            _endpoint = endpoint;
        }

        public async Task<TItem?> CreateAsync(TItem item, CancellationToken cancellationToken = default)
        {
            try
            {
                var result = await _client.PostAsJsonAsync(_endpoint, item, cancellationToken);

                if (result.IsSuccessStatusCode)
                {
                    return await result.Content.ReadFromJsonAsync<TItem>(cancellationToken: cancellationToken);
                }
            }
            catch
            {
                WeakReferenceMessenger.Default.Send<ServerErrorMessage>();
                return default;
            }

            return default;
        }

        public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            try
            {
                var result = await _client.DeleteAsync($"{_endpoint}/{id}", cancellationToken);

                return result.IsSuccessStatusCode;
            }
            catch
            {
                WeakReferenceMessenger.Default.Send<ServerErrorMessage>();
                return false;
            }
        }

        public async IAsyncEnumerable<TItem> GetAllAsync([EnumeratorCancellation]CancellationToken cancellationToken = default)
        {
            TItem[]? resultArray = null;

            try
            {
                var result = await _client.GetAsync(_endpoint, cancellationToken);

                if (result.IsSuccessStatusCode)
                {
                    resultArray = await result.Content.ReadFromJsonAsync<TItem[]>(cancellationToken: cancellationToken);
                }
            }
            catch
            {
                WeakReferenceMessenger.Default.Send<ServerErrorMessage>();
                yield break;
            }

            if (resultArray == null)
                yield break;

            foreach (var item in resultArray)
                yield return item;
        }

        public async Task<TItem?> GetItemAsync(Guid id, CancellationToken cancellationToken = default)
        {
            try
            {
                var result = await _client.GetAsync($"{_endpoint}/{id}", cancellationToken);

                if (result.IsSuccessStatusCode)
                {
                    return await result.Content.ReadFromJsonAsync<TItem>(cancellationToken: cancellationToken);
                }
            }
            catch
            {
                WeakReferenceMessenger.Default.Send<ServerErrorMessage>();
                return default;
            }

            return default;
        }

        public async Task<TItem?> UpdateAsync(TItem item, CancellationToken cancellationToken = default)
        {
            try
            {
                var result = await _client.PostAsJsonAsync(_endpoint, item, cancellationToken);

                if (result.IsSuccessStatusCode)
                {
                    return await result.Content.ReadFromJsonAsync<TItem>(cancellationToken: cancellationToken);
                }
            }
            catch
            {
                WeakReferenceMessenger.Default.Send<ServerErrorMessage>();
                return default;
            }

            return default;
        }
    }
}
