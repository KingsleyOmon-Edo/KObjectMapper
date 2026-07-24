namespace KObjectMapper;

/// <summary>
/// Represents a mapping configuration between a source and target type.
/// </summary>
public sealed class MappingTypeMap
{
    private readonly Dictionary<string, string> _customMemberMappings = new();
    private readonly HashSet<string> _ignoredMembers = new();

    public MappingTypeMap(Type sourceType, Type targetType)
    {
        ArgumentNullException.ThrowIfNull(sourceType);
        ArgumentNullException.ThrowIfNull(targetType);

        SourceType = sourceType;
        TargetType = targetType;
    }

    public Type SourceType { get; }

    public Type TargetType { get; }

    /// <summary>
    /// Gets the custom member mappings where the key is the source member name
    /// and the value is the target member name.
    /// </summary>
    public IReadOnlyDictionary<string, string> CustomMemberMappings => _customMemberMappings;

    /// <summary>
    /// Gets the set of target member names that should be ignored during mapping.
    /// </summary>
    public IReadOnlySet<string> IgnoredMembers => _ignoredMembers;

    internal void AddCustomMemberMapping(string sourceMemberName, string targetMemberName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sourceMemberName);
        ArgumentException.ThrowIfNullOrWhiteSpace(targetMemberName);

        _customMemberMappings[sourceMemberName] = targetMemberName;
    }

    internal void AddIgnoredMember(string targetMemberName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(targetMemberName);

        _ignoredMembers.Add(targetMemberName);
    }
}

