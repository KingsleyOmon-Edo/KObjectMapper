namespace KObjectMapper.Collections;

public sealed class CollectionMappingOptions<TSource, TTarget>
{
    public CollectionMergeMode MergeMode { get; private set; } = CollectionMergeMode.Replace;
    public Func<TSource, object>? SourceKeySelector { get; private set; }
    public Func<TTarget, object>? TargetKeySelector { get; private set; }

    public CollectionMappingOptions<TSource, TTarget> WithMergeMode(CollectionMergeMode mode)
    {
        MergeMode = mode;
        return this;
    }

    public CollectionMappingOptions<TSource, TTarget> WithKeySelector(
        Func<TSource, object> sourceKeySelector,
        Func<TTarget, object> targetKeySelector)
    {
        SourceKeySelector = sourceKeySelector;
        TargetKeySelector = targetKeySelector;
        return this;
    }
}
