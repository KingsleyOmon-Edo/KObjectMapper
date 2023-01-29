using System.Reflection;

namespace KObjectMapper.Abstractions;

public interface IObjectMutator
{
    void WriteToProperties(object source, object target, List<PropertyInfo> diffs);
}