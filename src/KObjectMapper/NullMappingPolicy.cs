namespace KObjectMapper;

/// <summary>
/// Defines how null source values are handled during mapping.
/// </summary>
public enum NullMappingPolicy
{
    /// <summary>
    /// Null source values are propagated to the target member (default behavior).
    /// </summary>
    Propagate,

    /// <summary>
    /// Null source values are ignored; the existing target member value is preserved.
    /// </summary>
    Ignore,

    /// <summary>
    /// Null source values are replaced with a configured substitute value.
    /// </summary>
    Substitute
}
