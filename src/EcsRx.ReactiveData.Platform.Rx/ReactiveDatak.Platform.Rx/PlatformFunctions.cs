using System;
using System.Reactive.Linq;

namespace EcsRx.ReactiveData.Platform.Rx
{
    public class PlatformFunctions : IPlatformFunctions
    {
        public static void Initialize()
        {
            StaticPlatformFunctions.PlatformFunctions = new PlatformFunctions();
        }

        public IObservable<T> Empty<T>()
        {
            return Observable.Empty<T>();
        }

        public IObservable<T> StartWith<T>(IObservable<T> source, T value)
        {
            return Observable.StartWith<T>(source, new T[] { value });
        }

        public IObservable<TResult> Select<TSource, TResult>(IObservable<TSource> source, Func<TSource, TResult> selector)
        {
            return source.Select(selector);
        }

        public IDisposable Subscribe<T>(IObservable<T> observable, Action<T> onNext)
        {
            return observable.Subscribe(onNext);
        }
    }
}