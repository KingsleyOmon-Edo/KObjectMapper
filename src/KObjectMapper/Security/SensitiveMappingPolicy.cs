namespace KObjectMapper.Security;

public enum SensitiveMappingPolicy
{
    ExcludeMarked,  // Only [Sensitive]-marked members excluded
    DefaultDeny     // All members excluded unless explicitly allowed
}
