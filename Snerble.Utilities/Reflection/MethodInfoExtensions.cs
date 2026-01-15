using JetBrains.Annotations;
using System.Reflection;

namespace Snerble.Utilities.Reflection;

[PublicAPI]
public static class MethodInfoExtensions
{
	public static MethodInfo Recast(this MethodInfo self, params Type[] types) => self.GetGenericMethodDefinition().MakeGenericMethod(types);
	public static MethodInfo Recast<T>(this MethodInfo self) => Recast(self, typeof(T));
	public static MethodInfo Recast<T1, T2>(this MethodInfo self) => Recast(self, typeof(T1), typeof(T2));
	public static MethodInfo Recast<T1, T2, T3>(this MethodInfo self) => Recast(self, typeof(T1), typeof(T2), typeof(T3));
	public static MethodInfo Recast<T1, T2, T3, T4>(this MethodInfo self) => Recast(self, typeof(T1), typeof(T2), typeof(T3), typeof(T4));
}