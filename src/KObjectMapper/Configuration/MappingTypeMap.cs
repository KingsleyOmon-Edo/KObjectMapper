using KObjectMapper.Abstractions;
using KObjectMapper.Security;

namespace KObjectMapper.Configuration;

/// <summary>
/// Represents a mapping configuration between a source and target type.
/// </summary>
public sealed class MappingTypeMap
{
    private readonly Dictionary<string, string> _customMemberMappings = new();
    private readonly HashSet<string> _ignoredMembers = new();
    private readonly Dictionary<string, object?> _nullSubstitutes = new();
    private readonly List<ITypeConverterBox> _converters = [];
    private readonly HashSet<string> _allowedMembers = new();

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
    /// Gets or sets the null mapping policy for this type map.
    /// </summary>
    public NullMappingPolicy? NullPolicy { get; internal set; }

    /// <summary>
    /// Gets the per-member null substitute values keyed by target member name.
    /// </summary>
    public IReadOnlyDictionary<string, object?> NullSubstitutes => _nullSubstitutes;

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

    internal void AddNullSubstitute(string targetMemberName, object? substituteValue)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(targetMemberName);

        _nullSubstitutes[targetMemberName] = substituteValue;
    }

    /// <summary>
    /// Gets the per-map type converters registered for this type map.
    /// </summary>
    internal IReadOnlyList<ITypeConverterBox> Converters => _converters.AsReadOnly();

    internal void AddConverter(ITypeConverterBox converter)
    {
        ArgumentNullException.ThrowIfNull(converter);
        _converters.Add(converter);
    }

    public bool UseSourceGeneration { get; internal set; }

    public SensitiveMappingPolicy? PerMapSensitivePolicy { get; internal set; }

    public IReadOnlySet<string> AllowedMembers => _allowedMembers;

    internal void AddAllowedMember(string memberName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(memberName);
        _allowedMembers.Add(memberName);
    }
}

