using JetBrains.Annotations;
using System.Reflection;

namespace Snerble.Utilities.Reflection;

public abstract class MemberInfoWrapper : MemberInfo
{
	/// <summary>
	/// Gets the underlying <see cref="MemberInfo"/> that is wrapped by this object.
	/// </summary>
	public abstract MemberInfo UnderlyingMember { get; }

	public override string Name => UnderlyingMember.Name;
	public override MemberTypes MemberType => MemberTypes.Custom;
	public override Type? DeclaringType => UnderlyingMember.DeclaringType;
	public override Type? ReflectedType => UnderlyingMember.ReflectedType;
	public override IEnumerable<CustomAttributeData> CustomAttributes => UnderlyingMember.CustomAttributes;
	public override int MetadataToken => UnderlyingMember.MetadataToken;
	public override Module Module => UnderlyingMember.Module;

	public override bool Equals(object? obj) => UnderlyingMember.Equals(obj);
	public override int GetHashCode() => UnderlyingMember.GetHashCode();
	public override string? ToString() => UnderlyingMember.ToString();
	public override IList<CustomAttributeData> GetCustomAttributesData() => UnderlyingMember.GetCustomAttributesData();
	public override object[] GetCustomAttributes(bool inherit) => UnderlyingMember.GetCustomAttributes(inherit);

	public override object[] GetCustomAttributes(Type attributeType, bool inherit) =>
		UnderlyingMember.GetCustomAttributes(attributeType, inherit);

	public override bool IsDefined(Type attributeType, bool inherit) =>
		UnderlyingMember.IsDefined(attributeType, inherit);
}

/// <summary>
/// Union type between <see cref="FieldInfo"/> and <see cref="PropertyInfo"/>.
/// </summary>
[PublicAPI]
public class DataMember : MemberInfoWrapper
{
	// ReSharper disable once ConvertToPrimaryConstructor
	public DataMember(FieldInfo field)
	{
		UnderlyingMember = field ?? throw new ArgumentNullException(nameof(field));
	}

	public DataMember(PropertyInfo property)
	{
		UnderlyingMember = property ?? throw new ArgumentNullException(nameof(property));
	}

	public override MemberInfo UnderlyingMember { get; }

	/// <summary>
	/// Gets the member type of the <see cref="UnderlyingMember"/>.
	/// </summary>
	public new MemberTypes MemberType => UnderlyingMember.MemberType;

	/// <summary>
	/// Gets whether <see cref="UnderlyingMember"/> is an instance of <see cref="FieldInfo"/>.
	/// </summary>
	public bool IsField => UnderlyingMember is FieldInfo;

	/// <summary>
	/// Gets whether <see cref="UnderlyingMember"/> is an instance of <see cref="PropertyInfo"/>.
	/// </summary>
	public bool IsProperty => UnderlyingMember is PropertyInfo;

	/// <inheritdoc cref="FieldInfo.FieldType"/>
	public Type DataType => Get(f => f.FieldType, p => p.PropertyType);

	/// <inheritdoc cref="FieldInfo.IsStatic"/>
	public bool IsStatic => Get(f => f.IsStatic, p => p.GetMethod?.IsStatic ?? p.SetMethod.IsStatic);

	/// <summary>
	/// Gets a value indicating whether this member's getter is public.
	/// </summary>
	public bool IsPublicGet => Get(f => f.IsPublic, p => p.GetMethod?.IsPublic ?? false);

	/// <summary>
	/// Gets a value indicating whether this member's setter is public.
	/// </summary>
	public bool IsPublicSet => Get(f => f.IsPublic, p => p.SetMethod?.IsPublic ?? false);

	/// <summary>
	/// Gets a value indicating whether this member's getter is private.
	/// </summary>
	public bool IsPrivateGet => Get(f => f.IsPrivate, p => p.GetMethod?.IsPrivate ?? false);

	/// <summary>
	/// Gets a value indicating whether this member's setter is private.
	/// </summary>
	public bool IsPrivateSet => Get(f => f.IsPrivate, p => p.SetMethod?.IsPrivate ?? false);

	/// <inheritdoc cref="PropertyInfo.CanRead"/>
	public bool CanRead => Get(f => true, p => p.CanRead);

	/// <inheritdoc cref="PropertyInfo.CanWrite"/>
	public bool CanWrite => Get(f => !f.IsInitOnly, p => p.CanWrite);

	/// <inheritdoc cref="FieldInfo.SetValue(object,object)"/>
	public void SetValue(object target, object value) =>
		Get<Action<object, object>>(
				f => f.SetValue,
				p => p.SetValue)
			(target, value);

	/// <inheritdoc cref="FieldInfo.GetValue"/>
	public object GetValue(object obj) =>
		Get<Func<object, object>>(
				f => f.GetValue,
				p => p.GetValue)
			(obj);

	/// <summary>
	/// Returns <see cref="UnderlyingMember"/> as a <see cref="FieldInfo"/>, or <see langword="null"/> if it isn't a field.
	/// </summary>
	public FieldInfo? AsField() => UnderlyingMember as FieldInfo;

	/// <summary>
	/// Returns <see cref="UnderlyingMember"/> as a <see cref="FieldInfo"/>, or <see langword="null"/> if it isn't a field.
	/// </summary>
	public PropertyInfo? AsProperty() => UnderlyingMember as PropertyInfo;

	/// <summary>
	/// Returns a value from either the underlying <see cref="FieldInfo"/> or <see cref="PropertyInfo"/>.
	/// </summary>
	/// <typeparam name="T">The type of the returned value.</typeparam>
	/// <param name="fieldGetter">A delegate that returns an equivalent value from the field.</param>
	/// <param name="propertyGetter">A delegate that returns an equivalent value from the property.</param>
	/// <exception cref="Exception">
	/// Thrown only if the <see cref="UnderlyingMember"/> is not a field or property.
	/// </exception>
	public T Get<T>(Func<FieldInfo, T> fieldGetter, Func<PropertyInfo, T> propertyGetter) => UnderlyingMember switch
	{
		FieldInfo field => fieldGetter(field),
		PropertyInfo property => propertyGetter(property),
		_ => throw new("Invalid state")
	};

	/// <summary>
	/// Creates a new <see cref="DataMember"/> from the given <paramref name="member"/>.
	/// </summary>
	/// <param name="member">An instance of <see cref="FieldInfo"/> or <see cref="PropertyInfo"/>.</param>
	/// <exception cref="ArgumentException">
	/// <paramref name="member"/> is not a <see cref="FieldInfo"/> or <see cref="PropertyInfo"/>.
	/// </exception>
	public static DataMember FromMember(MemberInfo member) => member switch
	{
		FieldInfo field => new(field),
		PropertyInfo property => new(property),
		_ => throw new ArgumentException("Only fields and properties valid data members")
	};

	public static implicit operator DataMember(FieldInfo field) => new(field);

	public static implicit operator DataMember(PropertyInfo property) => new(property);

	public static explicit operator FieldInfo(DataMember member) => (FieldInfo)member.UnderlyingMember;

	public static explicit operator PropertyInfo(DataMember member) => (PropertyInfo)member.UnderlyingMember;
}