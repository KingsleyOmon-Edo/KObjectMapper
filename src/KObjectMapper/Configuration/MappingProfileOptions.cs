using System.Reflection;
using KObjectMapper.Abstractions;
using KObjectMapper.Security;

namespace KObjectMapper.Configuration;

public sealed class MappingProfileOptions
{
    private readonly List<Type> _profileTypes = [];
    private readonly List<Assembly> _assemblies = [];
    private readonly List<ITypeConverterBox> _globalConverters = [];
    private readonly List<(Type, Type, object)> _asyncConverters = [];

    public Action<MappingError>? OnMappingError { get; private set; }
    public Action<MappingResult>? OnMappingCompleted { get; private set; }

    public MappingProfileOptions WithOnMappingError(Action<MappingError> hook)
    {
        OnMappingError = hook;
        return this;
    }

    public MappingProfileOptions WithOnMappingCompleted(Action<MappingResult> hook)
    {
        OnMappingCompleted = hook;
        return this;
    }

    /// <summary>
    /// Gets the global null mapping policy applied to all type maps that do not define their own policy.
    /// </summary>
    public NullMappingPolicy? GlobalNullPolicy { get; private set; }

    /// <summary>
    /// Gets a value indicating whether strict mapping mode is enabled.
    /// When enabled, mapping a type pair with no registered type map throws at runtime.
    /// </summary>
    public bool IsStrictMode { get; private set; }

    public bool UseSourceGeneration { get; private set; }

    public SensitiveMappingPolicy SensitivePolicy { get; private set; } = SensitiveMappingPolicy.ExcludeMarked;

    public MappingProfileOptions SetSensitivePolicy(SensitiveMappingPolicy policy)
    {
        SensitivePolicy = policy;
        return this;
    }

    /// <summary>
    /// Enables strict mapping mode. In strict mode, calling Map for a type pair that has no
    /// registered type map throws an <see cref="InvalidOperationException"/> immediately.
    /// </summary>
    /// <returns>The options instance for fluent chaining.</returns>
    public MappingProfileOptions EnableStrictMode()
    {
        IsStrictMode = true;
        return this;
    }

    public MappingProfileOptions EnableSourceGeneration()
    {
        UseSourceGeneration = true;
        return this;
    }

    /// <summary>
    /// Sets the global null mapping policy for all type maps.
    /// </summary>
    /// <param name="policy">The null mapping policy to apply globally.</param>
    /// <returns>The options instance for fluent chaining.</returns>
    public MappingProfileOptions SetGlobalNullPolicy(NullMappingPolicy policy)
    {
        GlobalNullPolicy = policy;
        return this;
    }

    public MappingProfileOptions AddProfile<TProfile>()
        where TProfile : MappingProfile
    {
        _profileTypes.Add(typeof(TProfile));

        return this;
    }

    public MappingProfileOptions ScanAssembly(Assembly assembly)
    {
        ArgumentNullException.ThrowIfNull(assembly);

        _assemblies.Add(assembly);

        return this;
    }

    /// <summary>
    /// Registers a global type converter available to all type maps.
    /// </summary>
    public MappingProfileOptions AddConverter<TSource, TTarget>(ITypeConverter<TSource, TTarget> converter)
    {
        ArgumentNullException.ThrowIfNull(converter);
        _globalConverters.Add(new TypeConverterBox<TSource, TTarget>(converter));
        return this;
    }

    public GraphMappingOptions GraphOptions { get; private set; } = new();

    public MappingProfileOptions ConfigureGraph(Action<GraphMappingOptions> configure)
    {
        configure(GraphOptions);
        return this;
    }

    internal IReadOnlyList<ITypeConverterBox> GetGlobalConverters() => _globalConverters.AsReadOnly();

    public MappingProfileOptions AddAsyncConverter<TSource, TTarget>(IAsyncTypeConverter<TSource, TTarget> converter)
    {
        ArgumentNullException.ThrowIfNull(converter);
        _asyncConverters.Add((typeof(TSource), typeof(TTarget), converter));
        return this;
    }

    internal IReadOnlyList<(Type, Type, object)> GetAsyncConverters() => _asyncConverters.AsReadOnly();

    internal IReadOnlyCollection<Type> GetProfileTypes()
    {
        HashSet<Type> profileTypes = new(_profileTypes);

        foreach (Assembly assembly in _assemblies)
        {
            Type[] assemblyTypes = assembly.GetTypes();

            foreach (Type assemblyType in assemblyTypes)
            {
                if (!typeof(MappingProfile).IsAssignableFrom(assemblyType) ||
                    assemblyType.IsAbstract ||
                    !assemblyType.IsClass ||
                    (!assemblyType.IsPublic && !assemblyType.IsNestedPublic))
                {
                    continue;
                }

                profileTypes.Add(assemblyType);
            }
        }

        return profileTypes.ToArray();
    }
}
