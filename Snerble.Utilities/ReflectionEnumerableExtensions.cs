using Snerble.Utilities.Reflection;
using System.Reflection;

namespace Snerble.Utilities;

public static class ReflectionEnumerableExtensions
{
	public static IEnumerable<T> WithAttribute<T>(this IEnumerable<T> e, Type attribute) where T : MemberInfo
	{
		return e.Where(x => Attribute.IsDefined(x, attribute));
	}
	public static IEnumerable<T> WithoutAttribute<T>(this IEnumerable<T> e, Type attribute) where T : MemberInfo
	{
		return e.Where(x => !Attribute.IsDefined(x, attribute));
	}

	public static IEnumerable<(DataMember, T)> FindAttribute<T>(this IEnumerable<DataMember> e) where T : Attribute
	{
		return e.Select(x => (x, x.GetCustomAttribute<T>())).Where(x => x.Item2 != null)!;
	}
	public static IEnumerable<(DataMember, T)> FindAttributes<T>(this IEnumerable<DataMember> e) where T : Attribute
	{
		return e.Select(x => (x, x.GetCustomAttribute<T>())).Where(x => x.Item2 != null)!;
	}
}
