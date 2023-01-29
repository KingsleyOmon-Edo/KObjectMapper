using System.Reflection;

namespace KObjectMapper.Abstractions;

public interface IMutationStrategy
{
    void WriteToProperties(object source, object target, List<PropertyInfo> diffs);
}