namespace KObjectMapper.Security;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
public sealed class SensitiveAttribute : Attribute
{
    public string? Reason { get; }
    public SensitiveAttribute(string? reason = null) { Reason = reason; }
}
