namespace Snerble.Utilities;

public static class TaskEnumerableExtensions
{
	extension<T>(IEnumerable<Task<T>> tasks)
	{
		/// <inheritdoc cref="Task.WhenAll{T}(IEnumerable{Task{T}})"/>
		public Task<T[]> WhenAll() => Task.WhenAll(tasks);

		/// <inheritdoc cref="Task.WhenAny{T}(IEnumerable{Task{T}})"/>
		public Task<T> WhenAny() => Task.WhenAny(tasks).Unwrap();

		/// <inheritdoc cref="Task.WhenEach{T}(IEnumerable{Task{T}})"/>
		public IAsyncEnumerable<Task<T>> WhenEach() => Task.WhenEach(tasks);
	}

	extension(IEnumerable<Task> tasks)
	{
		/// <inheritdoc cref="Task.WhenAll(IEnumerable{Task})"/>
		public Task WhenAll() => Task.WhenAll(tasks);

		/// <inheritdoc cref="Task.WhenAny(IEnumerable{Task})"/>
		public Task WhenAny() => Task.WhenAny(tasks).Unwrap();

		/// <inheritdoc cref="Task.WhenEach(IEnumerable{Task})"/>
		public IAsyncEnumerable<Task> WhenEach() => Task.WhenEach(tasks);
	}
}
