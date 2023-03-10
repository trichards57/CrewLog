using CrewLog.Shared.Interfaces;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace CrewLog.Api.Services
{
    public interface IServiceBase<TItem>
        where TItem : IIdentifiable
    {
        Task<bool> CheckExistsAsync(string ownerId, Guid id);

        Task<int> CountAsync(string ownerId);

        Task<int> CreateAsync(string ownerId, IEnumerable<TItem> items);

        Task<bool> DeleteIfExistsAsync(string ownerId, Guid id);

        IAsyncEnumerable<TItem> GetAllAsync(string ownerId);

        Task<TItem?> GetAsync(string ownerId, Guid id);

        Task<Guid?> UpsertAsync(string ownerId, TItem item);
    }

    internal abstract class ServiceBase<TItem> : IServiceBase<TItem>
        where TItem : IIdentifiable
    {
        private readonly CosmosClient _client;
        private readonly Container _container;
        private readonly Database _database;
        private readonly ILogger<ServiceBase<TItem>> _logger;

        private readonly Lazy<PartitionKey> _partitionKey = new(() => new PartitionKey(typeof(TItem).Name));

        public ServiceBase(ILogger<ServiceBase<TItem>> logger, CosmosClient cosmosClient, string database, string container)
        {
            _client = cosmosClient;
            _database = _client.GetDatabase(database);
            _container = _database.GetContainer(container);
            _logger = logger;
        }

        public virtual async Task<bool> CheckExistsAsync(string ownerId, Guid id)
        {
            if (string.IsNullOrEmpty(ownerId))
                throw new ArgumentException($"'{nameof(ownerId)}' cannot be null or empty.", nameof(ownerId));

            _logger.LogInformation("Checking For Item : {ItemName}, {ItemId}", typeof(TItem).Name, id);

            try
            {
                var item = await _container.ReadItemAsync<TItem>(id.ToString(), _partitionKey.Value);
                var belongsToUser = item.Resource.UserId.Equals(ownerId, StringComparison.InvariantCultureIgnoreCase);

                _logger.LogDebug("Item Found : {ItemName}, {ItemId}, {BelongsToUser}", typeof(TItem).Name, id, belongsToUser);

                return belongsToUser;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                _logger.LogDebug("Item Not Found : {ItemName}, {ItemId}", typeof(TItem).Name, id);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex, "Error accessing item : {ItemName}", typeof(TItem).Name);
                throw;
            }
        }

        public virtual async Task<int> CountAsync(string ownerId)
        {
            if (string.IsNullOrEmpty(ownerId))
                throw new ArgumentException($"'{nameof(ownerId)}' cannot be null or empty.", nameof(ownerId));

            _logger.LogInformation("Counting Items : {ItemName}", typeof(TItem).Name);

            try
            {
                var res = await _container.GetItemLinqQueryable<TItem>().Where(i => i.UserId.Equals(ownerId, StringComparison.InvariantCultureIgnoreCase)).CountAsync();

                _logger.LogDebug("Items Found : {ItemName}, {Count}", typeof(TItem).Name, res.Resource);

                return res;
            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex, "Error accessing item : {ItemName}", typeof(TItem).Name);
                throw;
            }
        }

        public virtual async Task<Guid> CreateAsync(string ownerId, TItem item)
        {
            if (string.IsNullOrEmpty(ownerId))
                throw new ArgumentNullException(nameof(ownerId));
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            _logger.LogInformation("Adding New Item : {ItemName}", typeof(TItem).Name);

            if (item.Id == Guid.Empty)
                item.Id = Guid.NewGuid();
            item.UserId = ownerId;

            try
            {
                var res = await _container.CreateItemAsync(item, _partitionKey.Value);

                return res.Resource.Id;
            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex, "Error adding item : {ItemName}", typeof(TItem).Name);
                throw;
            }
        }

        public virtual async Task<int> CreateAsync(string ownerId, IEnumerable<TItem> items)
        {
            if (string.IsNullOrEmpty(ownerId))
                throw new ArgumentNullException(nameof(ownerId));
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            _logger.LogInformation("Adding New Items : {ItemName}, {Count}", typeof(TItem).Name, items.Count());

            if (!items.Any())
                return 0;

            try
            {
                var trans = _container.CreateTransactionalBatch(_partitionKey.Value);

                foreach (var el in items)
                {
                    if (el.Id == Guid.Empty)
                        el.Id = Guid.NewGuid();
                    el.UserId = ownerId;

                    trans.CreateItem(el);
                }

                var res = await trans.ExecuteAsync();
                var count = res.Count(r => r.IsSuccessStatusCode);

                _logger.LogInformation("Successfully Added New Items : {ItemName}, {Count}", typeof(TItem).Name, count);
                return count;
            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex, "Error adding item : {ItemName}", typeof(TItem).Name);
                throw;
            }
        }

        public virtual async Task<bool> DeleteIfExistsAsync(string ownerId, Guid id)
        {
            if (string.IsNullOrEmpty(ownerId))
                throw new ArgumentNullException(nameof(ownerId));

            _logger.LogInformation("Deleting Item : {ItemName}", typeof(TItem).Name);

            try
            {
                var item = await _container.ReadItemAsync<TItem>(id.ToString(), _partitionKey.Value);
                var belongsToUser = item.Resource.UserId.Equals(ownerId, StringComparison.InvariantCultureIgnoreCase);

                _logger.LogDebug("Item Found : {ItemName}, {ItemId}, {BelongsToUser}", typeof(TItem).Name, id, belongsToUser);

                if (!belongsToUser)
                    return false;

                var res = await _container.DeleteItemAsync<TItem>(id.ToString(), _partitionKey.Value);

                return true;
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                _logger.LogDebug("Item Not Found : {ItemName}, {ItemId}", typeof(TItem).Name, id);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex, "Error accessing item : {ItemName}", typeof(TItem).Name);
                throw;
            }
        }

        public virtual async IAsyncEnumerable<TItem> GetAllAsync(string ownerId)
        {
            if (string.IsNullOrEmpty(ownerId))
                throw new ArgumentNullException(nameof(ownerId));

            _logger.LogInformation("Getting All Items : {ItemName}", typeof(TItem).Name);

            var res = _container.GetItemLinqQueryable<TItem>().Where(i => i.UserId.Equals(ownerId, StringComparison.InvariantCultureIgnoreCase)).ToFeedIterator();

            while (res.HasMoreResults)
            {
                foreach (var item in await res.ReadNextAsync())
                    yield return item;
            }
        }

        public virtual async Task<TItem?> GetAsync(string ownerId, Guid id)
        {
            if (string.IsNullOrEmpty(ownerId))
                throw new ArgumentException($"'{nameof(ownerId)}' cannot be null or empty.", nameof(ownerId));

            _logger.LogInformation("Getting Item : {ItemName}, {ItemId}", typeof(TItem).Name, id);

            try
            {
                var item = await _container.ReadItemAsync<TItem>(id.ToString(), _partitionKey.Value);
                var belongsToUser = item.Resource.UserId.Equals(ownerId, StringComparison.InvariantCultureIgnoreCase);

                _logger.LogDebug("Item Found : {ItemName}, {ItemId}, {BelongsToUser}", typeof(TItem).Name, id, belongsToUser);

                return belongsToUser ? item : default;
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                _logger.LogDebug("Item Not Found : {ItemName}, {ItemId}", typeof(TItem).Name, id);
                return default;
            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex, "Error accessing item : {ItemName}", typeof(TItem).Name);
                throw;
            }
        }

        public virtual async Task<Guid?> UpsertAsync(string ownerId, TItem item)
        {
            if (string.IsNullOrEmpty(ownerId))
                throw new ArgumentException($"'{nameof(ownerId)}' cannot be null or empty.", nameof(ownerId));

            if (item.UserId != ownerId)
                return null;

            _logger.LogInformation("Getting Item : {ItemName}, {ItemId}", typeof(TItem).Name, item.Id);

            try
            {
                var actualItem = await _container.ReadItemAsync<TItem>(item.Id.ToString(), _partitionKey.Value);
                var belongsToUser = actualItem.Resource.UserId.Equals(ownerId, StringComparison.InvariantCultureIgnoreCase);

                _logger.LogDebug("Item Found : {ItemName}, {ItemId}, {BelongsToUser}", typeof(TItem).Name, item.Id, belongsToUser);

                if (belongsToUser)
                {
                    await _container.UpsertItemAsync(item, _partitionKey.Value);
                    return item.Id;
                }

                return null;
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                await _container.UpsertItemAsync(item, _partitionKey.Value);
                return item.Id;
            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex, "Error accessing item : {ItemName}", typeof(TItem).Name);
                throw;
            }
        }
    }
}
