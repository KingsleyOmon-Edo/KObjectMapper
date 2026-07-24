namespace KObjectMapper.Abstractions;

/// <summary>
/// Non-generic marker interface used internally to store and look up typed converters.
/// </summary>
internal interface ITypeConverterBox
{
    Type SourceType { get; }
    Type TargetType { get; }
    object? ConvertObject(object? source);
}

/// <summary>
/// Wraps a strongly-typed <see cref="ITypeConverter{TSource,TTarget}"/> for non-generic dispatch.
/// </summary>
internal sealed class TypeConverterBox<TSource, TTarget> : ITypeConverterBox
{
    private readonly ITypeConverter<TSource, TTarget> _inner;

    public TypeConverterBox(ITypeConverter<TSource, TTarget> inner)
    {
        _inner = inner;
    }

    public Type SourceType => typeof(TSource);
    public Type TargetType => typeof(TTarget);

    public object? ConvertObject(object? source)
    {
        if (source is TSource typed)
        {
            return _inner.Convert(typed);
        }

        return source;
    }
}
