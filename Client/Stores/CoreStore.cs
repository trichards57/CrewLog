﻿using BlazorApp.Client.Services.Interfaces;
using BlazorApp.Shared.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel;

namespace BlazorApp.Client.Stores
{
    public interface IStore<TItem> : INotifyPropertyChanged
    { }

    public abstract class CoreStore<TItem> : ObservableObject, IStore<TItem>
        where TItem : IIdentifiable
    {
        private readonly IServiceBase<TItem> _service;
        private bool _isLoading;
        private DateTimeOffset _lastLoad = DateTimeOffset.MinValue;

        protected CoreStore(IServiceBase<TItem> service)
        {
            _service = service;
        }

        public Dictionary<Guid, TItem> Data { get; } = new Dictionary<Guid, TItem>();
        public bool IsLoading { get => _isLoading; set => SetProperty(ref _isLoading, value); }
        public bool IsLoadingItem(Guid id) => IsLoading;

        public Task LoadItems(IEnumerable<Guid> ids, bool force, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public Task Remove(Guid id, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public Task Add(TItem create, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public Task Put(TItem item, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }
    }

    public class CoreSubItemStore<TItem> : CoreStore<TItem>
        where TItem : IShiftIdentifiable
    {
        public CoreSubItemStore(IServiceBase<TItem> service) : base(service)
        {
        }

        public IEnumerable<TItem> GetShiftItems(Guid id) => Data.Values.Where(d => d.ShiftId == id);
    }

}