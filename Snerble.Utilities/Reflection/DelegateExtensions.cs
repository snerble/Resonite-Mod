using JetBrains.Annotations;

namespace Snerble.Utilities.Reflection;

[PublicAPI]
public static class DelegateExtensions
{
	/// <summary>
	/// Converts the given <paramref name="delegate"/> to another <paramref name="delegateType"/>.
	/// </summary>
	public static Delegate Convert(this Delegate @delegate, Type delegateType)
	{
		return Delegate.CreateDelegate(delegateType, @delegate.Target, @delegate.Method);
	}

	public static IEnumerable<TDelegate> EnumerateInvocationList<TDelegate>(this TDelegate @delegate) where TDelegate : Delegate
	{
		if (@delegate is not MulticastDelegate)
		{
			yield return @delegate;
			yield break;
		}

		var enumerator = Delegate.EnumerateInvocationList(@delegate);
		foreach (var invocation in enumerator) yield return invocation;
	}

	public static IEnumerable<TReturn> ExecuteInvocationList<TArg, TReturn>(this Func<TArg, TReturn> @delegate, TArg arg)
	{
		return @delegate.EnumerateInvocationList().Select(x => x.Invoke(arg));
	}
}