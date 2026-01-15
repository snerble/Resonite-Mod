using JetBrains.Annotations;

namespace Snerble.Utilities;

[PublicAPI]
public static class ObjectDictionaryExtensions
{
	public static bool TrySet<T>(this IDictionary<object, object?> dict) => dict.TryAdd(typeof(T), DBNull.Value);
	public static bool TrySet(this IDictionary<object, object?> dict, object key) => dict.TryAdd(key, DBNull.Value);

	public static T GetOrAdd<T>(this IDictionary<object, object?> dict) where T : new() => GetOrAdd(dict, typeof(T), () => new T());
	public static T GetOrAdd<T>(this IDictionary<object, object?> dict, Func<T> factory) where T : new() => GetOrAdd(dict, typeof(T), factory);
	public static T GetOrAdd<T>(this IDictionary<object, object?> dict, object key) where T : new() => GetOrAdd(dict, key, () => new T());
	public static T GetOrAdd<T>(this IDictionary<object, object?> dict, object key, Func<T> factory)
	{
		if (!dict.TryGetValue(key, out var value)) value = dict[key] = factory()!;
		return (T)value!;
	}
}
