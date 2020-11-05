using System;

namespace EcsRx.ReactiveData.Platform
{
    public interface IPlatformFunctions
    {
        IObservable<T> Empty<T>();

        IObservable<T> StartWith<T>(IObservable<T> source, T value);

        IObservable<TResult> Select<TSource, TResult>(IObservable<TSource> source, Func<TSource, TResult> selector);
        
        IDisposable Subscribe<T>(IObservable<T> observable, Action<T> onNext);
    }
}