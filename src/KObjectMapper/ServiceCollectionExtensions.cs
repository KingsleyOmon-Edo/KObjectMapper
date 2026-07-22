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
}
