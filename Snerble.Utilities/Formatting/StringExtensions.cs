using JetBrains.Annotations;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.RegularExpressions;

namespace Snerble.Utilities.Formatting;

[PublicAPI]
public static class StringExtensions
{
	private static readonly Regex NumberRegex = new(@"^([0-9]+)$", RegexOptions.Compiled);

	/// <summary>
	/// Performs a basic pattern match on <paramref name="s"/> using the same syntax as the SQL 'LIKE' operator.
	/// </summary>
	public static bool Like(this string? s, string pattern, RegexOptions options = RegexOptions.IgnoreCase )
	{
		if (s == null) return false;

		pattern = Regex.Replace(pattern, @"[-\[\]{}()+.,\\^$|#]", @"\$&");
		pattern = pattern
			.Replace("*", ".*")
			.Replace("?", ".");

		return Regex.IsMatch(s, pattern, options);
	}

	/// <summary>
	/// Joins all non-null elements in the enumerable into one string using the given seperator.
	/// </summary>
	public static string JoinNotNull<T>(this IEnumerable<T> items, string? separator) => items.Where(x => x != null).Join(separator);

	/// <summary>
	/// Joins the enumerable into one string using the given seperator.
	/// </summary>
	public static string Join<T>(this IEnumerable<T> items, string? separator)
	{
		var args = items.Where(x => x != null).ToArray();
		return args.Length == 0 ? string.Empty : string.Join(separator, args);
	}

	/// <summary>
	/// Joins the values in the enumerable with commas and the last element gets the word "and".
	/// </summary>
	public static string JoinEnumeration<T>(this IEnumerable<T> items)
	{
		var args = items.Where(x => x != null).ToArray();

		return args.Length switch
		{
			0 => string.Empty,
			<= 2 => string.Join(" and ", args),
			_ => string.Join(", ", args.SkipLast(1)) + ", and " + args.Last()
		};
	}

	/// <summary>
	/// Removes the byte-order mark (or preamble) from the encoded data.
	/// </summary>
	/// <param name="encoding"></param>
	/// <param name="data"></param>
	/// <returns></returns>
	public static byte[] StripPreamble(this Encoding encoding, byte[] data)
	{
		var preamble = encoding.Preamble;

		if (data.Length < preamble.Length)
			return data;

		for (int i = 0; i < preamble.Length; i++)
		{
			if (data[i] != preamble[i])
				return data;
		}

		return data[preamble.Length..];
	}

	/// <summary>
	/// Returns if a string is composed only of numbers.
	/// </summary>
	public static bool IsNumeric(this string s)
	{
		return NumberRegex.IsMatch(s);
	}

	/// <summary>
	/// Attempts to replace a sequence of <paramref name="a"/> with <paramref name="b"/>. If this results
	/// in an empty string, then the original string is returned instead.
	/// </summary>
	/// <param name="s">The string to modify.</param>
	/// <param name="a">The sequence to replace.</param>
	/// <param name="b">The substituting string.</param>
	/// <param name="stringComparison">The string comparison rules to use.</param>
	public static string TryReplace(this string s, string a, string b, StringComparison stringComparison = default)
	{
		var result = s.Replace(a, b, stringComparison);
		return string.IsNullOrEmpty(result) ? s : result;
	}

	/// <summary>
	/// Replaces all line endings (cr-lf or lf) with lf.
	/// </summary>
	public static string NormalizeLineEndings(this string s) => s.ReplaceLineEndings("\n");

	/// <summary>
	/// Splits a string by line endings, regardless of platform.
	/// </summary>
	public static string[] SplitLines(this string s, StringSplitOptions splitOptions = default) => s.NormalizeLineEndings().Split('\n', splitOptions);

	/// <summary>
	/// Adds quotes to a string. If the string already has quotes, then it won't be modified.
	/// </summary>
	public static string Quote(this string s)
	{
		if (s.Length >= 2 && s.StartsWith('"') && s.EndsWith('"')) return s;

		return string.Create(s.Length + 2, s, static (span, state) =>
		{
			span[0] = '"';
			state.CopyTo(span[1..]);
			span[^1] = '"';
		});
	}

	/// <summary>
	/// Removes the leading and trailing quote from a string. If the string isn't quoted,
	/// then it is returned as-is.
	/// </summary>
	public static string Unquote(this string s)
	{
		return s.Length >= 2 && s.StartsWith('"') && s.EndsWith('"') ? s[1..^1] : s;
	}

	/// <inheritdoc cref="string.IsNullOrEmpty"/>
	public static bool IsNullOrEmpty([NotNullWhen(false)] this string? s) => string.IsNullOrEmpty(s);

	/// <inheritdoc cref="string.IsNullOrWhiteSpace"/>
	public static bool IsNullOrWhitespace([NotNullWhen(false)] this string? s) => string.IsNullOrWhiteSpace(s);

	public static string? NullIfEmpty(this string? s) => s.IsNullOrEmpty() ? null : s;
	public static string? NullIfWhitespace(this string? s) => s.IsNullOrWhitespace() ? null : s;

	[return: NotNullIfNotNull(nameof(s))]
	public static string? RemovePrefix(this string? s, string? prefix)
	{
		if (string.IsNullOrEmpty(s) || string.IsNullOrEmpty(prefix) || !s.StartsWith(prefix)) return s;
		return s[prefix.Length..];
	}

	[return: NotNullIfNotNull(nameof(s))]
	public static string? RemoveSuffix(this string? s, string? suffix)
	{
		if (string.IsNullOrEmpty(s) || string.IsNullOrEmpty(suffix) || !s.EndsWith(suffix)) return s;
		return s[..^suffix.Length];
	}

	/// <summary>
	/// Removes the first prefix that <paramref name="s"/> starts with.
	/// </summary>
	[return: NotNullIfNotNull(nameof(s))]
	public static string? RemovePrefixes(this string? s, params string[] prefixes)
	{
		if (s == null) return null;
		foreach (var suffix in prefixes)
		{
			if (!s.StartsWith(suffix)) continue;
			return s[..^suffix.Length];
		}

		return s;
	}

	/// <summary>
	/// Removes the first suffix that <paramref name="s"/> ends with.
	/// </summary>
	[return: NotNullIfNotNull(nameof(s))]
	public static string? RemoveSuffixes(this string? s, params string[] suffixes)
	{
		if (s == null) return null;
		foreach (var suffix in suffixes)
		{
			if (!s.EndsWith(suffix)) continue;
			return s[..^suffix.Length];
		}

		return s;
	}

	private static readonly Regex PascalCaseRegex = new(@"([a-z]+|\d+|[A-Z][a-z]+|(?:[A-Z](?![A-Z][a-z]))+)", RegexOptions.Compiled);
	public static string[] SplitPascalCase(this string s) => PascalCaseRegex.Matches(s).Select(x => x.Value).ToArray();

	[return: NotNullIfNotNull(nameof(s))]
	public static string? Capitalize(this string? s)
	{
		if (s.IsNullOrEmpty()) return s;
		return string.Create(s.Length, s, static (span, s) =>
		{
			span[0] = char.ToUpper(s[0]);
			s.AsSpan(1).CopyTo(span[1..]);
		});
	}

	public static string Limit(this string s, int amount) => s[..Math.Clamp(amount, 0, s.Length)];
}