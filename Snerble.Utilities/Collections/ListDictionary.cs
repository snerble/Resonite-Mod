using JetBrains.Annotations;
using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Snerble.Utilities.Collections;

[PublicAPI]
public class ListDictionary<TKey, TValue> : ILookup<TKey, TValue> where TKey : notnull
{
	private class Group(TKey key, List<TValue> values) : IGrouping<TKey, TValue>
	{
		public TKey Key => key;
		public IEnumerator<TValue> GetEnumerator() => values.GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}

	private readonly Dictionary<TKey, List<TValue>> dict = [];

	public int Count => dict.Count;
	public ICollection<TKey> Keys => dict.Keys;
	public IEnumerable<TValue> this[TKey key] => dict[key];

	public bool Contains(TKey key) => dict.ContainsKey(key);
	
	public IReadOnlyList<TValue> Get(TKey key) => dict[key];
	public IReadOnlyList<TValue> GetValueOptional(TKey key) => dict.TryGetValue(key, out var value) ? value : [];
	public bool TryGetValue(TKey key, [NotNullWhen(true)] out IReadOnlyList<TValue>? value)
	{
		if (dict.TryGetValue(key, out var values))
		{
			value = values;
			return true;
		}

		value = null;
		return false;
	}

	public void Add(TKey key, TValue value)
	{
		if (dict.TryGetValue(key, out var list)) list.Add(value);
		else dict[key] = [value];
	}

	public bool Remove(TKey key) => dict.Remove(key);
	public bool Remove(TKey key, TValue value)
	{
		if (!dict.TryGetValue(key, out var list)) return false;
		if (!list.Remove(value)) return false;
		if (list.Count == 0) dict.Remove(key);
		return true;
	}

	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	public IEnumerator<IGrouping<TKey, TValue>> GetEnumerator()
	{
		return dict.Select(kv => new Group(kv.Key, kv.Value)).GetEnumerator();
	}
}
