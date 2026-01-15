using JetBrains.Annotations;

namespace Snerble.Utilities;

[PublicAPI]
public static class BoolExtensions
{
	public static int AsInt(this bool value) => value ? 1 : 0;
	public static float AsFloat(this bool value) => value ? 1 : 0;
	public static double AsDouble(this bool value) => value ? 1 : 0;
	public static decimal AsDecimal(this bool value) => value ? 1 : 0;
}
