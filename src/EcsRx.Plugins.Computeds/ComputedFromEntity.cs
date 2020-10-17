using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EcsRx.Entities;
using EcsRx.Events.Collections;
using EcsRx.Extensions;
using EcsRx.Groups.Observable;
using EcsRx.MicroRx.Extensions;
using EcsRx.MicroRx.Subjects;

namespace EcsRx.Plugins.Computeds
{
    /// <summary>
    /// Compute a singe value, trigger on entity add/remove
    /// Example use would be maintaining a dictionary of entities based on some criteria (AKA an index)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class ComputedFromEntity<T> : IComputed<T>, IDisposable
    {
        public List<IDisposable> Subscriptions { get; }
        
        public IObservableGroup InternalObservableGroup { get; }
        public T Value => MaybeRefreshGetData();
        
        private readonly Subject<T> _onDataChanged;
        private bool _needsUpdate;

        public ComputedFromEntity(IObservableGroup internalObservableGroup)
        {
            InternalObservableGroup = internalObservableGroup;
            Subscriptions = new List<IDisposable>();       
            
            _onDataChanged = new Subject<T>();

            RefreshData();
            MonitorChanges();
        }
        
        
        public IDisposable Subscribe(IObserver<T> observer)
        { return _onDataChanged.Subscribe(observer); }
        
        public void MonitorChanges()
        {
            InternalObservableGroup.OnEntityAdded.Subscribe(AddEntity).AddTo(Subscriptions);
            InternalObservableGroup.OnEntityRemoving.Subscribe(RemoveEntity).AddTo(Subscriptions);
            RefreshWhen()?.Subscribe(x => RequestUpdate()).AddTo(Subscriptions);
        }

        public void RequestUpdate(object _ = null)
        {
            _needsUpdate = true;
            
            if(_onDataChanged.HasObservers)
            { RefreshData(); }
        }

        public abstract void Reset();

        public abstract void AddEntity(IEntity entity);

        public abstract void RemoveEntity(IEntity entity);

        public abstract T GetData();

        public void RefreshData()
        {
            Reset();

            foreach (var entity in InternalObservableGroup)
            {
                AddEntity(entity);
            }

            _onDataChanged.OnNext(GetData());
            _needsUpdate = false;
        }

        /// <summary>
        /// The method to indicate when the listings should be updated
        /// </summary>
        /// <remarks>
        /// If there is no checking required outside of adding/removing this can
        /// return an empty observable, but common usages would be to refresh every update.
        /// The bool is throw away, but is a workaround for not having a Unit class
        /// </remarks>
        /// <returns>An observable trigger that should trigger when the group should refresh</returns>
        public abstract IObservable<bool> RefreshWhen();

        protected T MaybeRefreshGetData()
        {
            if (_needsUpdate)
            { RefreshData(); }

            return GetData();
        }

        public void Dispose()
        {
            Subscriptions.DisposeAll();
            _onDataChanged?.Dispose();
        }
    }
}