using System.Collections.Concurrent;
using System.Reflection;

namespace KObjectMapper;

public sealed class GeneratedMapperRegistry : IGeneratedMapperRegistry
{
    private readonly ConcurrentDictionary<(Type, Type), MethodInfo?> _cache = new();

    public bool TryMap(Type sourceType, Type targetType, object source, object target)
    {
        MethodInfo? method = _cache.GetOrAdd((sourceType, targetType), key => ResolveMethod(key.Item1, key.Item2));

        if (method is null)
        {
            return false;
        }

        method.Invoke(null, [source, target]);
        return true;
    }

    private static MethodInfo? ResolveMethod(Type sourceType, Type targetType)
    {
        string className = $"KObjectMapper.Generated.{sourceType.Name}To{targetType.Name}Mapper";

        foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            Type? mapperType = assembly.GetType(className);

            if (mapperType is null)
            {
                continue;
            }

            MethodInfo? method = mapperType.GetMethod("Map", BindingFlags.Public | BindingFlags.Static, [sourceType, targetType]);

            if (method is not null)
            {
                return method;
            }
        }

        return null;
    }
}
