using System.Reflection;

namespace KObjectMapper;

public class MutableWriter
{
    public MutableWriter()
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