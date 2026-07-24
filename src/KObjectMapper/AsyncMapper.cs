using KObjectMapper.Abstractions;

namespace KObjectMapper;

public sealed class AsyncMapper : IAsyncObjectMapper
{
    private readonly IObjectMapper _mapper;
    private readonly Dictionary<(Type, Type), object> _asyncConverters;

    public AsyncMapper(IObjectMapper mapper, IEnumerable<(Type, Type, object)> converters)
    {
        ArgumentNullException.ThrowIfNull(mapper);
        ArgumentNullException.ThrowIfNull(converters);
        _mapper = mapper;
        _asyncConverters = converters.ToDictionary(c => (c.Item1, c.Item2), c => c.Item3);
    }

    public void RegisterAsyncConverter<TSource, TTarget>(IAsyncTypeConverter<TSource, TTarget> converter)
    {
        ArgumentNullException.ThrowIfNull(converter);
        _asyncConverters[(typeof(TSource), typeof(TTarget))] = converter;
    }

    public async Task MapAsync(object source, object target, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(target);
        cancellationToken.ThrowIfCancellationRequested();

        Type sourceType = source.GetType();
        Type targetType = target.GetType();

        if (_asyncConverters.TryGetValue((sourceType, targetType), out object? conv))
        {
            Type converterType = typeof(IAsyncTypeConverter<,>).MakeGenericType(sourceType, targetType);
            System.Reflection.MethodInfo convertMethod = converterType.GetMethod("ConvertAsync")!;
            Task task = (Task)convertMethod.Invoke(conv, [source, cancellationToken])!;
            await task.ConfigureAwait(false);
            object result = ((dynamic)task).Result;
            _mapper.Map(result, target);
        }
        else
        {
            _mapper.Map(source, target);
        }
    }

    public async Task MapAsync<TSource, TTarget>(TSource source, TTarget target, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(target);
        cancellationToken.ThrowIfCancellationRequested();

        if (_asyncConverters.TryGetValue((typeof(TSource), typeof(TTarget)), out object? conv))
        {
            var converter = (IAsyncTypeConverter<TSource, TTarget>)conv;
            TTarget result = await converter.ConvertAsync(source, cancellationToken).ConfigureAwait(false);
            _mapper.Map(result, target);
        }
        else
        {
            _mapper.Map(source, target);
        }
    }

    public async Task<MappingResult> TryMapAsync(object source, object target, CancellationToken cancellationToken = default)
    {
        try
        {
            await MapAsync(source, target, cancellationToken).ConfigureAwait(false);
            return MappingResult.Success();
        }
        catch (OperationCanceledException)
        {
            return MappingResult.Failure(new MappingError { Reason = "Operation was cancelled." });
        }
        catch (Exception ex)
        {
            return MappingResult.Failure(new MappingError { Reason = ex.Message });
        }
    }

    public async Task<MappingResult> TryMapAsync<TSource, TTarget>(TSource source, TTarget target, CancellationToken cancellationToken = default)
    {
        try
        {
            await MapAsync(source, target, cancellationToken).ConfigureAwait(false);
            return MappingResult.Success();
        }
        catch (OperationCanceledException)
        {
            return MappingResult.Failure(new MappingError { Reason = "Operation was cancelled." });
        }
        catch (Exception ex)
        {
            return MappingResult.Failure(new MappingError { Reason = ex.Message });
        }
    }
}
