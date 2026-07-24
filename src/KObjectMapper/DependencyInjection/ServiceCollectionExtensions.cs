using System.Reflection;
using KObjectMapper;
using KObjectMapper.Abstractions;
using KObjectMapper.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace KObjectMapper.DependencyInjection;

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

        NullMappingPolicy? globalNullPolicy = options.GlobalNullPolicy;
        bool isStrictMode = options.IsStrictMode;
        IReadOnlyList<ITypeConverterBox> globalConverters = options.GetGlobalConverters();

        foreach (Type profileType in profileTypes)
        {
            services.AddTransient(typeof(MappingProfile), _ =>
                (MappingProfile)Activator.CreateInstance(profileType, nonPublic: true)!);
        }

        services.AddTransient<IObjectMapper>(sp =>
        {
            IEnumerable<MappingProfile> profiles = sp.GetServices<MappingProfile>();
            return new Mapper(profiles, globalNullPolicy, isStrictMode, globalConverters);
        });

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

            foreach (MappingTypeMap typeMap in profile.TypeMaps)
            {
                ValidateTypeMapCompleteness(profileType, typeMap, errors);
            }
        }

        if (errors.Count != 0)
        {
            throw new MappingProfileValidationException(errors);
        }
    }

    private static void ValidateTypeMapCompleteness(Type profileType, MappingTypeMap typeMap, List<string> errors)
    {
        ValidateTargetConstructor(profileType, typeMap, errors);

        PropertyInfo[] targetProperties = typeMap.TargetType
            .GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)
            .Where(p => p.CanWrite)
            .ToArray();

        PropertyInfo[] sourceProperties = typeMap.SourceType
            .GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)
            .Where(p => p.CanRead)
            .ToArray();

        foreach (PropertyInfo targetProperty in targetProperties)
        {
            if (typeMap.IgnoredMembers.Contains(targetProperty.Name))
            {
                continue;
            }

            bool hasCustomMapping = typeMap.CustomMemberMappings.Values.Contains(targetProperty.Name);

            if (hasCustomMapping)
            {
                continue;
            }

            bool hasConventionMapping = sourceProperties.Any(sp => sp.Name == targetProperty.Name);

            if (!hasConventionMapping)
            {
                errors.Add(
                    $"Profile '{profileType.FullName}' type map '{typeMap.SourceType.Name}' -> '{typeMap.TargetType.Name}' " +
                    $"has no mapping configured for required target member '{targetProperty.Name}'. " +
                    $"Either configure a custom mapping using ForMember, ignore it using Ignore, or ensure a source property with the same name exists.");
            }
        }
    }

    private static void ValidateTargetConstructor(Type profileType, MappingTypeMap typeMap, List<string> errors)
    {
        bool hasParameterlessConstructor = typeMap.TargetType
            .GetConstructors(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)
            .Any(ctor => ctor.GetParameters().Length == 0);

        if (!hasParameterlessConstructor)
        {
            errors.Add(
                $"Profile '{profileType.FullName}' type map '{typeMap.SourceType.Name}' -> '{typeMap.TargetType.Name}' " +
                $"has an ambiguous constructor binding: target type '{typeMap.TargetType.Name}' does not have a public parameterless constructor. " +
                $"Ensure the target type exposes a public parameterless constructor or configure explicit constructor mapping.");
        }
    }
}
