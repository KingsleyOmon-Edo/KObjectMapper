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

    protected MappingTypeMapConfiguration<TSource, TTarget> CreateMap<TSource, TTarget>()
    {
        MappingTypeMap typeMap = new(typeof(TSource), typeof(TTarget));
        _typeMaps.Add(typeMap);

        return new MappingTypeMapConfiguration<TSource, TTarget>(typeMap);
    }
}

