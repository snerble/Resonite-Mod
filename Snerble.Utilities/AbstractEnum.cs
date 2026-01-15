using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Snerble.Utilities;

public abstract class AbstractEnum<TEnum> : AbstractEnumBase<TEnum, string> where TEnum : AbstractEnum<TEnum>;

public abstract class AbstractEnumBase<TEnum, TValue> :
	IEquatable<AbstractEnumBase<TEnum, TValue>>,
	IComparable<AbstractEnumBase<TEnum, TValue>>,
	IEquatable<TValue>,
	IComparable<TValue>,
	IComparable
	where TEnum : AbstractEnumBase<TEnum, TValue>
	where TValue : IComparable
{
	private static Dictionary<TValue, TEnum>? _all;

	public abstract TValue Value { get; }

	public bool Equals(AbstractEnumBase<TEnum, TValue>? other)
	{
		if (other is null) return false;
		if (ReferenceEquals(this, other)) return true;
		return Value.Equals(other.Value);
	}

	public bool Equals(TValue? other)
	{
		if (other is null) return false;
		return Value.Equals(other);
	}

	public override bool Equals(object? obj)
	{
		if (obj is null) return false;
		if (ReferenceEquals(this, obj)) return true;
		if (obj is AbstractEnumBase<TEnum, TValue> @enum) return Equals(@enum);
		if (obj is TValue v) return Value.Equals(v);
		return false;
	}
	public override int GetHashCode() => Value.GetHashCode();
	public override string? ToString() => Value.ToString( );

	public int CompareTo(AbstractEnumBase<TEnum, TValue>? other)
	{
		if (other is null) return 1;
		if (ReferenceEquals(this, other)) return 0;
		return Value.CompareTo(other.Value);
	}

	public int CompareTo(TValue? other)
	{
		if (other is null) return 1;
		return Value.CompareTo(other);
	}

	public int CompareTo(object? obj)
	{
		if (obj is null) return 1;
		if (ReferenceEquals(this, obj)) return 0;
		if (obj is AbstractEnumBase<TEnum, TValue> other) return CompareTo(other);
		if (obj is TValue v) return CompareTo(v);
		throw new ArgumentException($"Object must be of type {nameof(AbstractEnumBase<TEnum, TValue>)} or {typeof(TValue)}");
	}

	public static ICollection<TEnum> List()
	{
		Initialize();
		return _all.Values;
	}

	public static TEnum FromValue(TValue value)
	{
		Initialize();
		return _all[value];
	}

	public static bool TryFromValue(TValue value, [NotNullWhen(true)] out TEnum? result)
	{
		Initialize();
		return _all.TryGetValue(value, out result);
	}

	[MemberNotNull(nameof(_all))]
	private static void Initialize()
	{
		if (_all != null) return;

		var type = typeof(TEnum);
		_all = type
			.GetFields(BindingFlags.Static | BindingFlags.Public)
			.Where(x => x.FieldType == type)
			.Select(x => (TEnum)x.GetValue(null)!)
			.ToDictionary(x => x.Value);
	}

	public static bool operator ==(AbstractEnumBase<TEnum, TValue>? left, AbstractEnumBase<TEnum, TValue>? right) => Equals(left, right);
	public static bool operator !=(AbstractEnumBase<TEnum, TValue>? left, AbstractEnumBase<TEnum, TValue>? right) => !Equals(left, right);
	public static bool operator <(AbstractEnumBase<TEnum, TValue>? left, AbstractEnumBase<TEnum, TValue>? right) => Comparer<AbstractEnumBase<TEnum, TValue>>.Default.Compare(left, right) < 0;
	public static bool operator >(AbstractEnumBase<TEnum, TValue>? left, AbstractEnumBase<TEnum, TValue>? right) => Comparer<AbstractEnumBase<TEnum, TValue>>.Default.Compare(left, right) > 0;
	public static bool operator <=(AbstractEnumBase<TEnum, TValue>? left, AbstractEnumBase<TEnum, TValue>? right) => Comparer<AbstractEnumBase<TEnum, TValue>>.Default.Compare(left, right) <= 0;
	public static bool operator >=(AbstractEnumBase<TEnum, TValue>? left, AbstractEnumBase<TEnum, TValue>? right) => Comparer<AbstractEnumBase<TEnum, TValue>>.Default.Compare(left, right) >= 0;

	public static implicit operator TValue(AbstractEnumBase<TEnum, TValue> @enum) => @enum.Value;
	public static implicit operator AbstractEnumBase<TEnum, TValue>(TValue v) => FromValue(v);
}