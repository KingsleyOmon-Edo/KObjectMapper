namespace KObjectMapper;

public interface IGeneratedMapperRegistry
{
    bool TryMap(Type sourceType, Type targetType, object source, object target);
}
