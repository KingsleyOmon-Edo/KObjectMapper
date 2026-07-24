using System.Reflection;

namespace KObjectMapper.Security;

public static class SensitiveDataGuard
{
    public static bool IsExcluded(
        PropertyInfo property,
        Type declaringType,
        SensitiveMappingPolicy policy,
        IReadOnlySet<string> allowedMembers)
    {
        bool propertyIsSensitive = property.GetCustomAttribute<SensitiveAttribute>() is not null;
        bool typeIsSensitive = declaringType.GetCustomAttribute<SensitiveAttribute>() is not null;

        return policy switch
        {
            SensitiveMappingPolicy.ExcludeMarked => propertyIsSensitive || typeIsSensitive,
            SensitiveMappingPolicy.DefaultDeny => !allowedMembers.Contains(property.Name),
            _ => false
        };
    }
}
