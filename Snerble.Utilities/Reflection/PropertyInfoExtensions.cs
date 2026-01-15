using JetBrains.Annotations;
using System.Reflection;
using System.Xml.Linq;

namespace Snerble.Utilities.Reflection;

[PublicAPI]
public static class PropertyInfoExtensions
{
	public static FieldInfo? GetBackingField(this PropertyInfo prop)
	{
		if (prop.SetMethod != null) return null;
		if (prop.GetMethod == null) return null;
		if (prop.DeclaringType == null) return null;

		var scope = prop.GetMethod.IsStatic ? BindingFlags.Static : BindingFlags.Instance;

		var name = $"<{prop.Name}>k__BackingField";
		return prop.DeclaringType.GetField(name, BindingFlags.NonPublic | scope);
	}
}

[PublicAPI]
public static class CustomAttributeExtensions
{
	/// <inheritdoc cref="Attribute.IsDefined(MemberInfo,Type)"/>
	public static bool IsDefined<T>(this MemberInfo member) => Attribute.IsDefined(member, typeof(T));
	/// <inheritdoc cref="Attribute.IsDefined(MemberInfo,Type,bool)"/>
	public static bool IsDefined<T>(this MemberInfo member, bool inherit) => Attribute.IsDefined(member, typeof(T), inherit);

	/// <inheritdoc cref="Attribute.IsDefined(ParameterInfo,Type)"/>
	public static bool IsDefined<T>(this ParameterInfo parameter) => Attribute.IsDefined(parameter, typeof(T));
	/// <inheritdoc cref="Attribute.IsDefined(ParameterInfo,Type,bool)"/>
	public static bool IsDefined<T>(this ParameterInfo parameter, bool inherit) => Attribute.IsDefined(parameter, typeof(T), inherit);
}