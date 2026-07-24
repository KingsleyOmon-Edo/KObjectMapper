namespace KObjectMapper.Abstractions;

/// <summary>
/// Defines a converter that transforms a value of type <typeparamref name="TSource"/> into a value of type <typeparamref name="TTarget"/>.
/// </summary>
/// <typeparam name="TSource">The source value type.</typeparam>
/// <typeparam name="TTarget">The target value type.</typeparam>
public interface ITypeConverter<in TSource, out TTarget>
{
    /// <summary>
    /// Converts the specified source value to the target type.
    /// </summary>
    /// <param name="source">The source value to convert.</param>
    /// <returns>The converted target value.</returns>
    TTarget Convert(TSource source);
}
