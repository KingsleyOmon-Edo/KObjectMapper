namespace KObjectMapper.Abstractions;

public interface IAsyncObjectMapper
{
    Task MapAsync(object source, object target, CancellationToken cancellationToken = default);
    Task MapAsync<TSource, TTarget>(TSource source, TTarget target, CancellationToken cancellationToken = default);
    Task<MappingResult> TryMapAsync(object source, object target, CancellationToken cancellationToken = default);
    Task<MappingResult> TryMapAsync<TSource, TTarget>(TSource source, TTarget target, CancellationToken cancellationToken = default);
}
