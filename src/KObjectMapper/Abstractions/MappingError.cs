namespace KObjectMapper.Abstractions;

public sealed class MappingError
{
    public string MemberPath { get; init; } = string.Empty;
    public Type? SourceType { get; init; }
    public Type? TargetType { get; init; }
    public string Reason { get; init; } = string.Empty;
}
