namespace KObjectMapper;

public abstract class MappingProfile
{
    private readonly List<MappingTypeMap> _typeMaps = [];

    protected MappingProfile()
    {
        Configure();
    }

    public IReadOnlyCollection<MappingTypeMap> TypeMaps => _typeMaps.AsReadOnly();

    protected abstract void Configure();

    protected void CreateMap<TSource, TTarget>()
    {
        _typeMaps.Add(new MappingTypeMap(typeof(TSource), typeof(TTarget)));
    }
}
