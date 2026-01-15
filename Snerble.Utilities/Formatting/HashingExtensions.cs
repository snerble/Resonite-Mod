using System.Security.Cryptography;
using System.Text;

// ReSharper disable InconsistentNaming

namespace Snerble.Utilities.Formatting;

public static class HashingExtensions
{
    public static byte[] GetEncoded(this string s, Encoding encoding) => encoding.GetBytes(s);
    public static byte[] Utf8Encoded(this string s) => s.GetEncoded(Encoding.UTF8);

    public static string MD5HashedHex(this string s) => s.Utf8Encoded().MD5HashedHex();
    public static string MD5HashedB64(this string s) => s.Utf8Encoded().MD5HashedB64();
    public static byte[] MD5Hashed(this string s) => s.Utf8Encoded().MD5Hashed();

    public static string MD5HashedHex(this byte[] bytes) => bytes.MD5Hashed().ToHexString();
    public static string MD5HashedB64(this byte[] bytes) => Convert.ToBase64String(bytes.MD5Hashed())[..^2];
    public static byte[] MD5Hashed(this byte[] bytes)
    {
        using var md5 = MD5.Create();
        var hash = md5.ComputeHash(bytes);
        return hash;
    }

    public static string ToHexString(this byte[] bytes)
    {
        return string.Join("", bytes.Select(x => x.ToString("x2")));
    }
}