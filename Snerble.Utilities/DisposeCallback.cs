using JetBrains.Annotations;

namespace Snerble.Utilities;

[PublicAPI]
public class DisposeCallback(Action callback) : IDisposable
{
	public static DisposeCallback Empty { get; } = new(static () => { });

	private bool disposed;

	public void Dispose()
	{
		if (disposed) return;
		disposed = true;

		callback();
	}
}

[PublicAPI]
public class AsyncDisposeCallback(Func<ValueTask> callback) : IAsyncDisposable
{
	private bool disposed;

	public ValueTask DisposeAsync()
	{
		if (disposed) return ValueTask.CompletedTask;
		disposed = true;

		return callback();
	}
}
