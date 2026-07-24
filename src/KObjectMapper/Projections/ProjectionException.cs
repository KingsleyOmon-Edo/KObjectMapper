namespace KObjectMapper.Projections;

public sealed class ProjectionException : Exception
{
    public Type SourceType { get; }
    public Type TargetType { get; }

    public ProjectionException(Type sourceType, Type targetType, string message)
        : base(message)
    {
        SourceType = sourceType;
        TargetType = targetType;
    }

    public ProjectionException(Type sourceType, Type targetType, string message, Exception inner)
        : base(message, inner)
    {
        SourceType = sourceType;
        TargetType = targetType;
    }
}
