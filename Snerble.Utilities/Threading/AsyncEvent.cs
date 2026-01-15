using JetBrains.Annotations;
using System.Collections.Immutable;

namespace Snerble.Utilities.Threading;

[PublicAPI]
public struct AsyncEvent<TDelegate>() where TDelegate : Delegate
{
	private readonly Lock syncRoot = new();

	public ImmutableArray<TDelegate> Subscriptions { get; private set; } = ImmutableArray<TDelegate>.Empty;

	public void Add(TDelegate callback)
	{
		lock(syncRoot) Subscriptions = Subscriptions.Add(callback);
	}

	public void Remove(TDelegate callback)
	{
		lock(syncRoot) Subscriptions = Subscriptions.Remove(callback);
	}

	public Task DynamicInvoke(params object?[]? args)
	{
		var results = Subscriptions.Select(x => x.DynamicInvoke(args));
		return Task.WhenAll(results.OfType<Task>());
	}
}

[PublicAPI]
public static class AsyncEventExtensions
{
	public static Task InvokeAsync(this AsyncEvent<Func<Task>> handler) => Task.WhenAll(handler.Subscriptions.Select(x => x()));
	public static Task InvokeAsync<T>(this AsyncEvent<Func<T, Task>> handler, T t) => Task.WhenAll(handler.Subscriptions.Select(x => x(t)));
	public static Task InvokeAsync<T1, T2>(this AsyncEvent<Func<T1, T2, Task>> handler, T1 t1, T2 t2) => Task.WhenAll(handler.Subscriptions.Select(x => x(t1, t2)));
	public static Task InvokeAsync<T1, T2, T3>(this AsyncEvent<Func<T1, T2, T3, Task>> handler, T1 t1, T2 t2, T3 t3) => Task.WhenAll(handler.Subscriptions.Select(x => x(t1, t2, t3)));
	public static Task InvokeAsync<T1, T2, T3, T4>(this AsyncEvent<Func<T1, T2, T3, T4, Task>> handler, T1 t1, T2 t2, T3 t3, T4 t4) => Task.WhenAll(handler.Subscriptions.Select(x => x(t1, t2, t3, t4)));
	public static Task InvokeAsync<T1, T2, T3, T4, T5>(this AsyncEvent<Func<T1, T2, T3, T4, T5, Task>> handler, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5) => Task.WhenAll(handler.Subscriptions.Select(x => x(t1, t2, t3, t4, t5)));
	public static Task InvokeAsync<T1, T2, T3, T4, T5, T6>(this AsyncEvent<Func<T1, T2, T3, T4, T5, T6, Task>> handler, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6) => Task.WhenAll(handler.Subscriptions.Select(x => x(t1, t2, t3, t4, t5, t6)));
}
