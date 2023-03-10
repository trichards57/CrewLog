using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using CrewLog.Client.Messages;
using CrewLog.Client.Services.Interfaces;
using CrewLog.Shared.Interfaces;
using System.ComponentModel;

namespace CrewLog.Client.Stores
{
    public interface IStore<TItem> : INotifyPropertyChanged
    {
        Dictionary<Guid, TItem> Data { get; }
        bool IsLoading { get; set; }

        Task<Guid> Add(TItem create, CancellationToken cancellationToken = default);

        bool IsLoadingItem(Guid id);

        Task LoadItems(IEnumerable<Guid>? ids = null, bool force = false, CancellationToken cancellationToken = default);

        Task Put(TItem item, CancellationToken cancellationToken = default);

        Task Remove(Guid id, CancellationToken cancellationToken = default);
    }

    public interface ISubItemStore<TItem> : IStore<TItem>
    {
        IEnumerable<TItem> GetShiftItems(Guid id);
    }

    public abstract class CoreStore<TItem> : ObservableObject, IStore<TItem>
        where TItem : IIdentifiable
    {
        private readonly NewDataItem _itemType;
        private readonly IServiceBase<TItem> _service;
        private bool _isLoading;
        private DateTimeOffset _lastLoad = DateTimeOffset.MinValue;

        protected CoreStore(IServiceBase<TItem> service, NewDataItem itemType)
        {
            _itemType = itemType;
            _service = service;
        }

        public Dictionary<Guid, TItem> Data { get; } = new Dictionary<Guid, TItem>();
        public bool IsLoading { get => _isLoading; set => SetProperty(ref _isLoading, value); }

        private void SendNewDataMessage(IEnumerable<Guid> ids)
        {
            WeakReferenceMessenger.Default.Send(new NewDataMessage
            {
                DataType = _itemType,
                Ids = ids
            });
        }

        private void SendNewDataMessage(params Guid[] ids)
        {
            SendNewDataMessage(ids.AsEnumerable());
        }

        public Task<Guid> Add(TItem create, CancellationToken cancellationToken = default)
        {
            create.Id = Guid.NewGuid();

            Data.Add(create.Id, create);

            _service.CreateAsync(create, cancellationToken).ContinueWith(c=>
            {
                if (!c.IsCompletedSuccessfully || c.Result == null)
                    Data.Remove(create.Id);
                else
                    Data[create.Id] = c.Result;

                SendNewDataMessage(create.Id);
            }, cancellationToken);

            SendNewDataMessage(create.Id);

            return Task.FromResult(create.Id);
        }

        public bool IsLoadingItem(Guid id) => IsLoading;

        public async Task LoadItems(IEnumerable<Guid>? ids = null, bool force = false, CancellationToken cancellationToken = default)
        {
            var timeSinceLastLoad = DateTimeOffset.UtcNow - _lastLoad;

            if (!force && timeSinceLastLoad.TotalMinutes < 5)
                return;

            IsLoading = true;

            var result = _service.GetAllAsync(cancellationToken);

            if (cancellationToken.IsCancellationRequested)
                return;

            Data.Clear();

            await foreach (var item in result)
                Data.Add(item.Id, item);

            IsLoading = false;
            _lastLoad = DateTimeOffset.UtcNow;

            SendNewDataMessage(Data.Keys.ToArray());
        }

        public Task Put(TItem item, CancellationToken cancellationToken = default)
        {
            if (Data.TryGetValue(item.Id, out var oldItem))
            {
                Data[item.Id] = item;

                _service.UpdateAsync(item, cancellationToken).ContinueWith(c =>
                {
                    if (!c.IsCompletedSuccessfully || c.Result == null)
                        Data[item.Id] = oldItem;
                    else
                        Data[item.Id] = c.Result;

                    SendNewDataMessage(item.Id);
                }, cancellationToken);
                    
                SendNewDataMessage(item.Id);
            }

            return Task.CompletedTask;
        }

        public Task Remove(Guid id, CancellationToken cancellationToken = default)
        {
            if (Data.TryGetValue(id, out var oldItem))
            {
                Data.Remove(id);

                _service.DeleteAsync(id, cancellationToken).ContinueWith(c =>
                {
                    if (!c.IsCompletedSuccessfully)
                    {
                        Data.Add(id, oldItem);
                        SendNewDataMessage(id);
                    }
                }, cancellationToken);

                SendNewDataMessage(id);
            }

            return Task.CompletedTask;
        }
    }

    public abstract class CoreSubItemStore<TItem> : CoreStore<TItem>, ISubItemStore<TItem>
        where TItem : IShiftIdentifiable
    {
        protected CoreSubItemStore(IServiceBase<TItem> service, NewDataItem itemType) 
            : base(service, itemType)
        {
        }

        public IEnumerable<TItem> GetShiftItems(Guid id) => Data.Values.Where(d => d.ShiftId == id);
    }
}
