using System.Reflection;

namespace KObjectMapper.Abstractions;

public interface IObjectMutationStrategy
{
    void WriteToProperties(object source, object target, List<PropertyInfo> diffs);
}