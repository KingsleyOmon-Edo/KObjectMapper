using System.Reflection;
using KObjectMapper.Abstractions;

namespace KObjectMapper.Helpers;

public class ImmutableTypesStrategy : IObjectMutationStrategy
{
    public void WriteToProperties(object source, object target, List<PropertyInfo> diffs)
    {
        //  Provide a working implementation for immutable types
        return;
    }
}