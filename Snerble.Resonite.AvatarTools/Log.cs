using System.ComponentModel;
using System.Runtime.CompilerServices;
using ResoniteModLoader;

// ReSharper disable once CheckNamespace
namespace Snerble.ResoAvatarTools;

internal static class Log
{
    #region Deprecated
    [Obsolete("Use the string interpolation overload")]
    public static void DebugFunc(Func<object> messageProducer) => ResoniteMod.DebugFunc(messageProducer);
    #endregion

    #region Debug
    public static void Debug(object message) => ResoniteMod.Debug(message);
    public static void Debug<T>(T message)
    {
        if (!ResoniteMod.IsDebugEnabled()) return;
        ResoniteMod.Debug(message?.ToString()!);
    }

    public static void Debug(params object[] messages) => ResoniteMod.Debug(messages);
    public static void Debug(params ReadOnlySpan<object?> messages)
    {
        if (!ResoniteMod.IsDebugEnabled()) return;
        foreach (var message in messages) ResoniteMod.Debug(message!);
    }

    public static void Debug(ref DebugLoggingInterpolatedStringHandler message)
    {
        if (!ResoniteMod.IsDebugEnabled()) return;
        ResoniteMod.Debug(message.ToString());
    }
    #endregion

    #region Info
    public static void Info(object message) => ResoniteMod.Msg(message);
    public static void Info<T>(T message) => ResoniteMod.Msg(message?.ToString()!);

    public static void Info(params object[] messages) => ResoniteMod.Msg(messages);
    public static void Info(params ReadOnlySpan<object?> messages) { foreach (var message in messages) ResoniteMod.Msg(message!); }

    public static void Info(ref DefaultInterpolatedStringHandler message) => ResoniteMod.Msg(message.ToString());
    #endregion

    #region Warn
    public static void Warn(object message) => ResoniteMod.Warn(message);
    public static void Warn<T>(T message) => ResoniteMod.Warn(message?.ToString()!);

    public static void Warn(params object[] messages) => ResoniteMod.Warn(messages);
    public static void Warn(params ReadOnlySpan<object?> messages) { foreach (var message in messages) ResoniteMod.Warn(message!); }

    public static void Warn(ref DefaultInterpolatedStringHandler message) => ResoniteMod.Warn(message.ToString());
    #endregion

    #region Error
    public static void Error(object message) => ResoniteMod.Error(message);
    public static void Error<T>(T message) => ResoniteMod.Error(message?.ToString()!);

    public static void Error(params object[] messages) => ResoniteMod.Error(messages);
    public static void Error(params ReadOnlySpan<object?> messages) { foreach (var message in messages) ResoniteMod.Error(message!); }

    public static void Error(ref DefaultInterpolatedStringHandler message) => ResoniteMod.Error(message.ToString());
    #endregion
}

[InterpolatedStringHandler]
[EditorBrowsable(EditorBrowsableState.Never)]
internal ref struct DebugLoggingInterpolatedStringHandler
{
    private DefaultInterpolatedStringHandler _handler;

    #region ctors
    public DebugLoggingInterpolatedStringHandler(int literalLength, int formattedCount, out bool isEnabled)
    {
        isEnabled = ResoniteMod.IsDebugEnabled();
        _handler = isEnabled ? new DefaultInterpolatedStringHandler(literalLength, formattedCount) : default;
    }
    public DebugLoggingInterpolatedStringHandler(int literalLength, int formattedCount, IFormatProvider? provider, out bool isEnabled)
    {
        isEnabled = ResoniteMod.IsDebugEnabled();
        _handler = isEnabled ? new DefaultInterpolatedStringHandler(literalLength, formattedCount, provider) : default;
    }
    public DebugLoggingInterpolatedStringHandler(int literalLength, int formattedCount, IFormatProvider? provider, Span<char> initialBuffer, out bool isEnabled)
    {
        isEnabled = ResoniteMod.IsDebugEnabled();
        _handler = isEnabled ? new DefaultInterpolatedStringHandler(literalLength, formattedCount, provider, initialBuffer) : default;
    }
    #endregion

    #region Default handler wrapping
    public void AppendFormatted(scoped ReadOnlySpan<char> value) => _handler.AppendFormatted(value);
    public void AppendFormatted(scoped ReadOnlySpan<char> value, int alignment, string? format = null) => _handler.AppendFormatted(value, alignment, format);
    public void AppendFormatted(string? value) => _handler.AppendFormatted(value);
    public void AppendFormatted(string? value, int alignment, string? format = null) => _handler.AppendFormatted(value, alignment, format);
    public void AppendFormatted<T>(T value, int alignment) => _handler.AppendFormatted(value);
    public void AppendFormatted<T>(T value, int alignment, string? format) => _handler.AppendFormatted(value, alignment, format);
    public void AppendFormatted<T>(T value, string? format) => _handler.AppendFormatted(value, format);
    public void AppendFormatted(object? value, int alignment = 0, string? format = null) => _handler.AppendFormatted(value, alignment, format);
    public void AppendFormatted<T>(T value) => _handler.AppendFormatted(value);
    public void AppendLiteral(string value) => _handler.AppendLiteral(value);
    public void Clear() => _handler.Clear();

    /// <inheritdoc cref="DefaultInterpolatedStringHandler.ToString"/>
    public override string ToString() => _handler.ToString();

    /// <inheritdoc cref="DefaultInterpolatedStringHandler.ToStringAndClear"/>
    public string ToStringAndClear() => _handler.ToStringAndClear();
    #endregion
}