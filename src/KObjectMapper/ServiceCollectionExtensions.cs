using KObjectMapper.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace KObjectMapper;

/// <summary>
/// Extension methods for registering KObjectMapper services in dependency injection containers.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers the default object mapper services.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <returns>The same service collection instance.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="services"/> is null.</exception>
    public static IServiceCollection AddKObjectMapper(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddTransient<IObjectMapper, Mapper>();

        return services;
    }

    /// <summary>
    /// Registers the default object mapper services and validates the configured mapping profiles.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <param name="configure">Configures the mapping profile options.</param>
    /// <returns>The same service collection instance.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="services"/> or <paramref name="configure"/> is null.</exception>
    /// <exception cref="MappingProfileValidationException">Thrown when a profile is missing type maps or declares duplicate maps.</exception>
    public static IServiceCollection AddKObjectMapper(this IServiceCollection services, Action<MappingProfileOptions> configure)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configure);

        MappingProfileOptions options = new();
        configure(options);

        IReadOnlyCollection<Type> profileTypes = options.GetProfileTypes();
        ValidateProfiles(profileTypes);

        services.AddTransient<IObjectMapper, Mapper>();

        foreach (Type profileType in profileTypes)
        {
            services.AddTransient(typeof(MappingProfile), _ =>
                (MappingProfile)Activator.CreateInstance(profileType, nonPublic: true)!);
        }

        return services;
    }

    private static void ValidateProfiles(IReadOnlyCollection<Type> profileTypes)
    {
        List<string> errors = [];

        foreach (Type profileType in profileTypes)
        {
            MappingProfile profile = (MappingProfile)Activator.CreateInstance(profileType, nonPublic: true)!;

            if (profile.TypeMaps.Count == 0)
            {
                errors.Add($"Profile '{profileType.FullName}' does not define any type maps.");
                continue;
            }

            IEnumerable<IGrouping<(Type SourceType, Type TargetType), MappingTypeMap>> duplicateMaps = profile.TypeMaps
                .GroupBy(typeMap => (typeMap.SourceType, typeMap.TargetType))
                .Where(group => group.Count() > 1);

            foreach (IGrouping<(Type SourceType, Type TargetType), MappingTypeMap> duplicateMap in duplicateMaps)
            {
                errors.Add(
                    $"Profile '{profileType.FullName}' declares duplicate type map '{duplicateMap.Key.SourceType.FullName}' -> '{duplicateMap.Key.TargetType.FullName}'.");
            }
        }

        if (errors.Count != 0)
        {
            throw new MappingProfileValidationException(errors);
        }
    }
}
