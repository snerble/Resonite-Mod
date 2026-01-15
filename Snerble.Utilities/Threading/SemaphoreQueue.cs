using JetBrains.Annotations;
using System.Collections.Concurrent;

namespace Snerble.Utilities.Threading;

/// <summary>
/// Represents a semaphore that limits the numer of threads that can access a resource, and unblocks them in a first-in first-out order (FIFO).
/// </summary>
[PublicAPI]
public class SemaphoreQueue : IDisposable
{
	private readonly SemaphoreSlim semaphore;
	private readonly ConcurrentQueue<TaskCompletionSource<bool>> queue = new();
	private bool disposed;

	public SemaphoreQueue(int initialCount) => semaphore = new(initialCount);
	public SemaphoreQueue(int initialCount, int maxCount) => semaphore = new(initialCount, maxCount);

	public int CurrentCount => semaphore.CurrentCount;
	public WaitHandle AvailableWaitHandle => semaphore.AvailableWaitHandle;

	public void Wait() => WaitAsync(Timeout.InfiniteTimeSpan, CancellationToken.None).GetAwaiter().GetResult();
	public void Wait(CancellationToken cancellationToken) => Wait(Timeout.InfiniteTimeSpan, cancellationToken);
	public bool Wait(TimeSpan timeout) => WaitAsync(timeout, CancellationToken.None).GetAwaiter().GetResult();
	public bool Wait(TimeSpan timeout, CancellationToken cancellationToken) => WaitAsync(timeout, cancellationToken).GetAwaiter().GetResult();

	public Task WaitAsync() => WaitAsync(Timeout.InfiniteTimeSpan, CancellationToken.None);
	public Task WaitAsync(CancellationToken cancellationToken) => WaitAsync(Timeout.InfiniteTimeSpan, cancellationToken);
	public Task<bool> WaitAsync(TimeSpan timeout) => WaitAsync(timeout, CancellationToken.None);
	public Task<bool> WaitAsync(TimeSpan timeout, CancellationToken cancellationToken)
	{
		var tcs = new TaskCompletionSource<bool>();
		queue.Enqueue(tcs);

		semaphore.WaitAsync(timeout, cancellationToken).ContinueWith(task =>
		{
			if (!queue.TryDequeue(out var popped)) return;
			if (task.IsCanceled) popped.SetCanceled(cancellationToken);
			else if (task.IsFaulted) popped.SetException(task.Exception);
			else popped.SetResult(task.Result);
		}, cancellationToken);

		return tcs.Task;
	}

	public int Release() => semaphore.Release();
	public int Release(int releaseCount) => semaphore.Release(releaseCount);

	public void Dispose()
	{
		if (disposed) return;
		disposed = true;

		semaphore.Dispose();

		foreach (var tcs in queue) tcs.SetException(new ObjectDisposedException(null));
		queue.Clear();
	}
}