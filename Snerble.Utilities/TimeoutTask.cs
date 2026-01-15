using System.Runtime.CompilerServices;

namespace Snerble.Utilities
{
	public struct TimeoutTask
	{
		private DateTime _created;

		public TimeoutTask(TimeSpan timeout)
		{
			_created = DateTime.UtcNow;
			Timeout = timeout;
		}

		public TimeSpan Timeout { get; set; }
		public TimeSpan Remaining => new(Math.Max((Timeout - (DateTime.UtcNow - _created)).Ticks, 0));

		public void Reset() => _created = DateTime.UtcNow;
		
		public TaskAwaiter GetAwaiter() => Task.Delay(Remaining).GetAwaiter();

		public ConfiguredCancelableTimeoutTask ConfigureAwait(bool continueOnCapturedContext) => new(this, continueOnCapturedContext, default);
		public ConfiguredCancelableTimeoutTask WithCancellation(CancellationToken cancellationToken) => new(this, true, cancellationToken);
	}

	public readonly struct ConfiguredCancelableTimeoutTask
	{
		private readonly TimeoutTask _timeout;
		private readonly CancellationToken _cancellationToken;
		private readonly bool _continueOnCapturedContext;

		internal ConfiguredCancelableTimeoutTask(TimeoutTask timeout, bool continueOnCapturedContext, CancellationToken cancellationToken)
		{
			_timeout = timeout;
			_continueOnCapturedContext = continueOnCapturedContext;
			_cancellationToken = cancellationToken;
		}

		public ConfiguredCancelableTimeoutTask ConfigureAwait(bool continueOnCapturedContext) =>
			new(_timeout, continueOnCapturedContext, _cancellationToken);
		public ConfiguredCancelableTimeoutTask WithCancellation(CancellationToken cancellationToken) =>
			new(_timeout, _continueOnCapturedContext, cancellationToken);

		public ConfiguredTaskAwaitable.ConfiguredTaskAwaiter GetAwaiter() => Task.Delay(_timeout.Remaining, _cancellationToken)
			.ConfigureAwait(_continueOnCapturedContext)
			.GetAwaiter();
	}
}
