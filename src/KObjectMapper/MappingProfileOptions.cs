using System.Reflection;

namespace KObjectMapper;

public sealed class MappingProfileOptions
{
    private readonly List<Type> _profileTypes = [];
    private readonly List<Assembly> _assemblies = [];

    /// <summary>
    /// Gets the global null mapping policy applied to all type maps that do not define their own policy.
    /// </summary>
    public NullMappingPolicy? GlobalNullPolicy { get; private set; }

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
