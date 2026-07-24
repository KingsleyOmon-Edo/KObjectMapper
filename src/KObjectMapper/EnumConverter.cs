using KObjectMapper.Abstractions;

namespace KObjectMapper;

/// <summary>
/// Provides factory methods for creating safe enum converters.
/// </summary>
public static class EnumConverter
{
    /// <summary>
    /// Creates a converter that safely converts a <see cref="string"/> to <typeparamref name="TEnum"/>.
    /// </summary>
    /// <typeparam name="TEnum">The target enum type.</typeparam>
    /// <param name="ignoreCase">When true, parsing is case-insensitive.</param>
    public static IEnumConverter<string, TEnum> FromString<TEnum>(bool ignoreCase = false)
        where TEnum : struct, Enum =>
        new StringEnumConverter<TEnum>(ignoreCase);

    /// <summary>
    /// Creates a converter that safely converts an <see cref="int"/> to <typeparamref name="TEnum"/>.
    /// </summary>
    /// <typeparam name="TEnum">The target enum type.</typeparam>
    public static IEnumConverter<int, TEnum> FromInt32<TEnum>()
        where TEnum : struct, Enum =>
        new Int32EnumConverter<TEnum>();

    // ── String → Enum ────────────────────────────────────────────────────────

    private sealed class StringEnumConverter<TEnum>(bool ignoreCase) : IEnumConverter<string, TEnum>
        where TEnum : struct, Enum
    {
        public EnumConversionResult<TEnum> Convert(string source)
        {
            if (Enum.TryParse<TEnum>(source, ignoreCase, out TEnum result))
            {
                return EnumConversionResult<TEnum>.Success(result);
            }

            return EnumConversionResult<TEnum>.Failure(
                $"Value '{source}' is not a valid member of enum '{typeof(TEnum).Name}'. " +
                $"Valid values are: {string.Join(", ", Enum.GetNames(typeof(TEnum)))}.");
        }

        public ITypeConverter<string, TEnum> AsTypeConverter() => new ThrowingConverter<string, TEnum>(this);
    }

    // ── Int32 → Enum ─────────────────────────────────────────────────────────

    private sealed class Int32EnumConverter<TEnum> : IEnumConverter<int, TEnum>
        where TEnum : struct, Enum
    {
        public EnumConversionResult<TEnum> Convert(int source)
        {
            object boxed = Enum.ToObject(typeof(TEnum), source);

            if (Enum.IsDefined(typeof(TEnum), boxed))
            {
                return EnumConversionResult<TEnum>.Success((TEnum)boxed);
            }

            return EnumConversionResult<TEnum>.Failure(
                $"Value '{source}' is not a defined numeric value of enum '{typeof(TEnum).Name}'. " +
                $"Valid values are: {string.Join(", ", Enum.GetValues<TEnum>().Select(v => $"{v} ({System.Convert.ToInt32(v, System.Globalization.CultureInfo.InvariantCulture)})"))}.");
        }

        public ITypeConverter<int, TEnum> AsTypeConverter() => new ThrowingConverter<int, TEnum>(this);
    }

    // ── Throwing wrapper ──────────────────────────────────────────────────────

    private sealed class ThrowingConverter<TSource, TEnum>(IEnumConverter<TSource, TEnum> inner)
        : ITypeConverter<TSource, TEnum>
        where TEnum : struct, Enum
    {
        public TEnum Convert(TSource source)
        {
            EnumConversionResult<TEnum> result = inner.Convert(source);

            if (!result.IsSuccess)
            {
                throw new InvalidOperationException(result.Error);
            }

            return result.Value;
        }
    }
}
