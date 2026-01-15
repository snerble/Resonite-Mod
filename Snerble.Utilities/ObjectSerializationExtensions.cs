using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Snerble.Utilities;

public static class ObjectSerializationExtensions
{
	[return: NotNullIfNotNull(nameof(obj))]
	public static IDictionary<string, object?>? ToDictionary(this object? obj)
	{
		return obj?.GetType()
			.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty)
			.ToDictionary(
				x => x.Name,
				x => x.GetValue(obj));
	}
}
