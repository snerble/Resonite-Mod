using JetBrains.Annotations;
using System.Reflection;
using System.Runtime.Serialization;

namespace Snerble.Utilities.Reflection;

[PublicAPI]
public static class EnumReflectionExtensions
{
	private static readonly Dictionary<dynamic, string> _nameCache = new();

	public static FieldInfo? GetField<T>(this T self) where T : Enum
	{
		string? name = Enum.GetName(typeof(T), self);
		return typeof(T).GetFields().Skip(1).FirstOrDefault(x => x.Name == name);
	}

	/// <summary>
	/// Returns the name contained in the <see cref="EnumMemberAttribute"/>, or the result of <see cref="Enum.ToString()"/>.
	/// </summary>
	public static string GetName<T>(this T self) where T : Enum
	{
		lock (_nameCache)
		{
			if (_nameCache.TryGetValue(self, out var result)) return result;

			var field = self.GetField();

			if (field == null) result = self.ToString();
			else result = field.GetCustomAttribute<EnumMemberAttribute>()?.Value ?? field.Name;

			return _nameCache[self] = result;
		}
	}
}