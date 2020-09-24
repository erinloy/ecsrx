using System;

namespace EcsRx.ReactiveData.Platform
{
    public interface IPlatformFunctions
    {
        IObservable<T> Empty<T>();

        IObservable<T> StartWith<T>(IObservable<T> source, T value);
    }
}