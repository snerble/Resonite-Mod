using JetBrains.Annotations;
using System.Reflection;
using System.Runtime.Serialization;

namespace Snerble.Utilities.Reflection;

[PublicAPI]
public static class DataMemberExtensions
{
	private const BindingFlags AllInstance = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
	private const BindingFlags Default = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public;

	public static IList<DataMember> GetDataMembers(this Type type, bool useAttributes)
	{
		if (!useAttributes) return type.GetDataMembers();

		var members = type.GetDataMembers(AllInstance);
		members.RemoveAll(x => x.IsPrivateGet && !x.IsDefined(typeof(DataMemberAttribute)));
		members.RemoveAll(x => x.IsDefined(typeof(IgnoreDataMemberAttribute)));
		return members;
	}

	public static List<DataMember> GetDataMembers(this Type type)
	{
		return type.GetDataMembers(Default);
	}

	public static List<DataMember> GetDataMembers(this Type type, BindingFlags bindingFlags)
	{
		var fields = type.GetFields(bindingFlags);
		var properties = type.GetProperties(bindingFlags);

		return fields.Select(x => new DataMember(x))
			.Concat(properties.Select(x => new DataMember(x)))
			.ToList();
	}

	public static DataMember? GetDataMember(this Type type, string name)
	{
		return type.GetDataMember(name, Default);
	}

	public static DataMember? GetDataMember(this Type type, string name, BindingFlags bindingFlags)
	{
		if (type.GetField(name, bindingFlags) is { } field) return new(field);
		if (type.GetProperty(name, bindingFlags) is { } prop) return new(prop);
		return null;
	}

	public static FieldInfo? GetBackingField(this DataMember member)
	{
		if (member.IsField) return member.AsField();

		var type = member.DeclaringType;
		if (type == null) return null;

		var flags = BindingFlags.NonPublic;
		flags |= member.IsStatic ? BindingFlags.Static : BindingFlags.Instance;

		return type.GetField($"<{member.Name}>k__BackingField", flags);
	}
}