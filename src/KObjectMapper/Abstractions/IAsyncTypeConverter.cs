namespace KObjectMapper.Abstractions;

public interface IAsyncTypeConverter<TSource, TTarget>
{
    Task<TTarget> ConvertAsync(TSource source, CancellationToken cancellationToken = default);
}
