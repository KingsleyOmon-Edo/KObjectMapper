using System.Reflection;
using KObjectMapper.Abstractions;

namespace KObjectMapper.Helpers;

public class MutableTypesStrategy : IObjectMutationStrategy
{
    public MutableTypesStrategy()
    {
    }
    
    public void WriteToProperties(object source, object target, List<PropertyInfo> diffs)
    {
        foreach (var sourceProp in diffs)
        {
            foreach (var targetProp in target.GetType().GetProperties())
            {
                if (sourceProp.Name == targetProp.Name
                    && sourceProp.GetValue(source) != targetProp.GetValue(target))
                {
                    targetProp.SetValue(target, sourceProp.GetValue(source));
                }
            }
        }
    }
}