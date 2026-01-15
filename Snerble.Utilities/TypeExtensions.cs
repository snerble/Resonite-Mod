using Snerble.Utilities.Formatting;
using System.Runtime.CompilerServices;
using System.Text;

namespace Snerble.Utilities
{
    public static class TypeExtensions
    {
        public static Type? GetImplementation(this Type type, Type genericType)
        {
            if (genericType.IsAssignableFrom(type)) return type;
            if (genericType.IsGenericType == false) return null;

            return type.GetBaseClasses()
                .Concat(type.GetInterfaces())
                .FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == genericType);
        }

        public static Type? GetListElementType(this Type? type)
        {
            var impl = type?.GetImplementation(typeof(IEnumerable<>));
            return impl?.GetGenericArguments()[0];
        }

        public static IEnumerable<Type> GetBaseClasses(this Type? type)
        {
            while (type != null)
            {
                yield return type;
                type = type.BaseType;
            }
        }

        public static IEnumerable<Type> GetEnclosingTypes(this Type? type)
        {
            while (type != null)
            {
                yield return type;
                type = type.DeclaringType;
            }
        }

        public static string GetSimpleAssemblyQualifiedName(this Type type) => $"{type.FullName}, {type.Assembly.GetName().Name}";

        public static bool IsAnonymousType(this Type type)
        {
            var hasCompilerGeneratedAttribute = type.GetCustomAttributes(typeof(CompilerGeneratedAttribute), false).Any();
            var nameContainsAnonymousType = type.FullName?.Contains("AnonymousType") ?? false;

            return hasCompilerGeneratedAttribute && nameContainsAnonymousType;
        }

        private static readonly Dictionary<Type, string> CommonTypeNameLookup = new()
        {
            [typeof(bool)] = "bool",

            [typeof(sbyte)] = "sbyte",
            [typeof(short)] = "short",
            [typeof(int)] = "int",
            [typeof(long)] = "long",

            [typeof(byte)] = "byte",
            [typeof(ushort)] = "ushort",
            [typeof(uint)] = "uint",
            [typeof(ulong)] = "ulong",

            [typeof(char)] = "char",
            [typeof(string)] = "string",

            [typeof(float)] = "float",
            [typeof(double)] = "double",
            [typeof(decimal)] = "decimal"
        };

        /// <summary>
        /// Returns the type name with the generic type arguments filled in.
        /// </summary>
        public static string GetName(this Type type) => GetDisplay(type, TypeDisplayOptions.Short);

        /// <summary>
        /// Returns the full type name with the generic type arguments filled in.
        /// </summary>
        public static string GetFullName(this Type type) => GetDisplay(type, TypeDisplayOptions.Long);

        /// <summary>
        /// Returns the type name including the parts specified by <paramref name="options"/>.
        /// </summary>
        public static string GetDisplay(this Type type, TypeDisplayOptions options)
        {
            if (options == TypeDisplayOptions.None) return "";
            if (type.IsGenericParameter) return type.Name;
            if (options == TypeDisplayOptions.QualifiedLong) return type.AssemblyQualifiedName!;

            // Map builtin type keywords
            if (options.HasFlag(TypeDisplayOptions.TypeKeywords) && CommonTypeNameLookup.TryGetValue(type, out var keyword)) return keyword;

            var sb = new StringBuilder();

            // Append type name
            if (options.HasFlag(TypeDisplayOptions.Name)) sb.Append(type.Name);

            // Append generic args tag (replaces the standard `1)
            if (options.HasFlag(TypeDisplayOptions.GenericArguments) && type.IsGenericType)
            {
                var typeArgs = type.GetGenericArguments().Select(x => x.GetDisplay(options)).ToArray();

                if (sb.Length != 0)
                {
                    var trailLength = (int)Math.Log10(typeArgs.Length) + 2; // +1 to include 0-9, and another for the backtick (`)
                    sb.Remove(sb.Length - trailLength, trailLength);
                }

                sb.Append('<');
                for (var i = 0; i < typeArgs.Length; i++)
                {
                    if (i != 0) sb.Append(", ");
                    sb.Append(typeArgs[i]);
                }
                sb.Append('>');
            }

            // Prepend nested types
            if (options.HasFlag(TypeDisplayOptions.NestedTypes) && type.DeclaringType != null)
            {
                var enclosing = type.DeclaringType.GetDisplay(options & ~TypeDisplayOptions.Assembly);
                sb.Insert(0, '+');
                sb.Insert(0, enclosing);
                options &= ~TypeDisplayOptions.Namespace;
            }

            // Prepend namespace
            if (options.HasFlag(TypeDisplayOptions.Namespace))
            {
                sb.Insert(0, '.');
                sb.Insert(0, type.Namespace);
            }

            // Append assembly parts
            if ((options & TypeDisplayOptions.Assembly) == TypeDisplayOptions.Assembly) sb.Append($", {type.Assembly.FullName}");
            else if ((options & TypeDisplayOptions.Assembly) != default)
            {
                var name = type.Assembly.GetName();
                if (options.HasFlag(TypeDisplayOptions.AssemblyName)) sb.Append($", {name.Name}");
                if (options.HasFlag(TypeDisplayOptions.AssemblyVersion)) sb.Append($", Version={name.Version}");
                if (options.HasFlag(TypeDisplayOptions.AssemblyCulture)) sb.Append($", Culture={name.CultureName switch
                {
                    null or "" => "neutral",
                    { } culture => culture
                }}");
                if (options.HasFlag(TypeDisplayOptions.AssemblyPublicKey)) sb.Append($", PublicKeyToken={name.GetPublicKeyToken() switch
                {
                    null or { Length: 0 } => "null",
                    { } token => token.ToHexString()
                }}");
            }

            return sb.ToString();
        }

        /// <summary>
        /// Returns the underlying type of the nullable type, or returns <paramref name="type"/>
        /// if it isn't a <see cref="Nullable{T}"/>.
        /// </summary>
        public static Type GetUnderlyingType(this Type type) => Nullable.GetUnderlyingType(type) ?? type;
    }
}

[Flags]
public enum TypeDisplayOptions
{
    None = 0,

    Short = Name | GenericArguments | TypeKeywords,
    Long = Namespace | NestedTypes | Short,

    QualifiedShort = (Long | AssemblyName) & ~(TypeKeywords | GenericArguments),
    QualifiedLong = QualifiedShort | Assembly,

    Namespace = 1 << 0,
    NestedTypes = 1 << 1,
    Name = 1 << 2,
    TypeKeywords = 1 << 3,
    GenericArguments = 1 << 4,

    Assembly = AssemblyName | AssemblyVersion | AssemblyCulture | AssemblyPublicKey,
    AssemblyName = 1 << 5,
    AssemblyVersion = 1 << 6,
    AssemblyCulture = 1 << 7,
    AssemblyPublicKey = 1 << 8,
}