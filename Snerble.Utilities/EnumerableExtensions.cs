using JetBrains.Annotations;
using System.Collections;

namespace Snerble.Utilities;

[PublicAPI]
public static class EnumerableExtensions
{
	/// <summary>
	/// Consumes the enumerator.
	/// </summary>
	public static void Enumerate(this IEnumerator e)
	{
		while (e.MoveNext()) { }
	}

	/// <summary>
	/// Consumes the enumerable.
	/// </summary>
	public static void Enumerate(this IEnumerable e)
	{
		var enumerator = e.GetEnumerator();
		using (enumerator as IDisposable) enumerator.Enumerate();
	}

	/// <summary>
	/// Invokes a delegate for each element in the enumerable.
	/// </summary>
	/// <param name="enumerable">The collection to enumerate.</param>
	/// <param name="action">The delegate to invoke per element.</param>
	public static void ForEach<T>(this IEnumerable<T> enumerable, [InstantHandle] Action<T> action)
	{
		foreach (var item in enumerable) action(item);
	}

	/// <inheritdoc cref="ForEach{T}(IEnumerable{T},Action{T})"/>
	public static void ForEach<T>(this IEnumerable<T> enumerable, [InstantHandle] Action<T, int> action)
	{
		var i = 0;
		foreach (var element in enumerable) action(element, i++);
	}

	private class Single<T>(T value) : IReadOnlyList<T>
	{
		private class SingleEnumerator(T value) : IEnumerator<T>
		{
			private int pos = -1;

			public T Current => pos == 0 ? value : default!;
			object IEnumerator.Current => Current!;

			public bool MoveNext() => ++pos == 0;

			public void Reset() => pos = -1;
			public void Dispose() { }
		}

		public IEnumerator<T> GetEnumerator() => new SingleEnumerator(value);
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		public int Count => 1;
		public T this[int index] => index == 0 ? value : throw new ArgumentOutOfRangeException(nameof(index));
	}
	
	/// <summary>
	/// Wraps the object in a read-only list.
	/// </summary>
	public static IReadOnlyList<T> Yield<T>(this T t) => new Single<T>(t);
}