using System.Reflection;

namespace KObjectMapper;

public sealed class MappingProfileOptions
{
    private readonly List<Type> _profileTypes = [];
    private readonly List<Assembly> _assemblies = [];

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
