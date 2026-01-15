using System.Collections;

namespace Snerble.Utilities;

public static class NullEnumerableExtensions
{
	public static IEnumerable<T> EmptyIfNull<T>(this IEnumerable<T>? self) => self ?? [];
	public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T?> e) where T : class => e.Where(x => x != null)!;
	public static IEnumerable<T?> WhereNull<T>(this IEnumerable<T?> e) where T : class => e.Where(x => x == null);

	public static T? NullIfEmpty<T>(this T self) where T : ICollection => self.Count == 0 ? default : self;
}