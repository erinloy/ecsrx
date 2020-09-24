using EcsRx.ReactiveData.Platform;
using System;
using System.Reactive.Linq;

namespace EcsRx.ReactiveData.Platform.Rx
{
	public class PlatformFunctions : IPlatformFunctions
	{
		public PlatformFunctions()
		{
		}

		public IObservable<T> Empty<T>()
		{
			return Observable.Empty<T>();
		}

		public static void Initialize()
		{
			StaticPlatformFunctions.PlatformFunctions = new PlatformFunctions();
		}

		public IObservable<T> StartWith<T>(IObservable<T> source, T value)
		{
			return Observable.StartWith<T>(source, new T[] { value });
		}
	}
}