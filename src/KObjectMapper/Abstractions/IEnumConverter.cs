namespace KObjectMapper.Abstractions;

/// <summary>
/// Defines a safe enum converter that returns a structured result instead of throwing on invalid input.
/// </summary>
/// <typeparam name="TSource">The source value type (e.g. <see cref="string"/> or <see cref="int"/>).</typeparam>
/// <typeparam name="TEnum">The target enum type.</typeparam>
public interface IEnumConverter<TSource, TEnum> where TEnum : struct, Enum
{
    /// <summary>
    /// Converts the source value to the target enum, returning a structured result.
    /// </summary>
    EnumConversionResult<TEnum> Convert(TSource source);

    /// <summary>
    /// Returns an <see cref="ITypeConverter{TSource,TEnum}"/> that wraps this converter and throws
    /// <see cref="InvalidOperationException"/> when conversion fails.
    /// </summary>
    ITypeConverter<TSource, TEnum> AsTypeConverter();
}
