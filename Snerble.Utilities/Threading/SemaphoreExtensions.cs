using JetBrains.Annotations;

namespace Snerble.Utilities.Threading;

[PublicAPI]
public static class SemaphoreExtensions
{
	#region SemaphoreSlim
	public static async Task<IDisposable> WaitDisposableAsync(this SemaphoreSlim sem)
	{
		await sem.WaitAsync();
		return new DisposeCallback(() => sem.Release());
	}

	public static async Task<IDisposable> WaitDisposableAsync(this SemaphoreSlim sem, CancellationToken cancellationToken)
	{
		await sem.WaitAsync(cancellationToken);
		return new DisposeCallback(() => sem.Release());
	}

	public static async Task<SemaphoreWaitResult> WaitDisposableAsync(this SemaphoreSlim sem, TimeSpan timeout)
	{
		return new(sem.Release, await sem.WaitAsync(timeout));
	}

	public static async Task<SemaphoreWaitResult> WaitDisposableAsync(this SemaphoreSlim sem, TimeSpan timeout, CancellationToken cancellationToken)
	{
		return new(sem.Release, await sem.WaitAsync(timeout, cancellationToken));
	}
	#endregion

	#region SemaphoreQueue
	public static async Task<IDisposable> WaitDisposableAsync(this SemaphoreQueue sem)
	{
		await sem.WaitAsync();
		return new DisposeCallback(() => sem.Release());
	}

	public static async Task<IDisposable> WaitDisposableAsync(this SemaphoreQueue sem, CancellationToken cancellationToken)
	{
		await sem.WaitAsync(cancellationToken);
		return new DisposeCallback(() => sem.Release());
	}

	public static async Task<SemaphoreWaitResult> WaitDisposableAsync(this SemaphoreQueue sem, TimeSpan timeout)
	{
		return new(sem.Release, await sem.WaitAsync(timeout));
	}

	public static async Task<SemaphoreWaitResult> WaitDisposableAsync(this SemaphoreQueue sem, TimeSpan timeout, CancellationToken cancellationToken)
	{
		return new(sem.Release, await sem.WaitAsync(timeout, cancellationToken));
	} 
	#endregion
}

public struct SemaphoreWaitResult(Func<int> release, bool lockTaken) : IDisposable
{
	private Func<int>? release = release;

	public bool LockTaken { get; } = lockTaken;

	public void Dispose()
	{
		if (!LockTaken || release == null) return;

		release();
		release = null;
	}

	public static implicit operator bool(SemaphoreWaitResult result) => result.LockTaken;
}