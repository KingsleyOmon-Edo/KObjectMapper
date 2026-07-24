using KObjectMapper;
using KObjectMapper.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace KObjectMapper.UnitTests.CharacterizationTests;

public class DependencyInjectionTests
{
    [Fact]
    public void AddKObjectMapper_WhenCalled_RegistersIObjectMapperAsTransientMapper()
    {
        IServiceCollection services = new ServiceCollection();

        services.AddKObjectMapper();

        ServiceDescriptor descriptor = services.Single(sd => sd.ServiceType == typeof(IObjectMapper));
        descriptor.ImplementationType.ShouldBe(typeof(Mapper));
        descriptor.Lifetime.ShouldBe(ServiceLifetime.Transient);
    }
}
