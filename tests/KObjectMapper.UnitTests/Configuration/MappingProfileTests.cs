namespace KObjectMapper.UnitTests.Configuration;

public class MappingProfileTests
{
    [Fact]
    public void MappingProfile_RegistersTypeMapDefinitions_WhenConfigureIsCalled()
    {
        MappingProfile profile = new CustomerMappingProfile();

        profile.TypeMaps.ShouldContain(typeMap =>
            typeMap.SourceType == typeof(CustomerSource) &&
            typeMap.TargetType == typeof(CustomerTarget));
    }

    [Fact]
    public void AddKObjectMapper_ScansAssemblyForProfiles_RegistersEveryProfileType()
    {
        ServiceCollection services = new();

        services.AddKObjectMapper(options => options.ScanAssembly(typeof(CustomerMappingProfile).Assembly));

        IServiceProvider provider = services.BuildServiceProvider();
        IReadOnlyCollection<MappingProfile> profiles = provider.GetServices<MappingProfile>().ToArray();

        profiles.ShouldContain(profile => profile.GetType() == typeof(CustomerMappingProfile));
        profiles.ShouldContain(profile => profile.GetType() == typeof(OrderMappingProfile));
    }

    [Fact]
    public void AddKObjectMapper_AddProfile_ThrowsWhenProfileDefinesNoTypeMaps()
    {
        ServiceCollection services = new();

        Should.Throw<MappingProfileValidationException>(() =>
            services.AddKObjectMapper(options => options.AddProfile<EmptyMappingProfile>()));
    }

    public sealed class CustomerMappingProfile : MappingProfile
    {
        protected override void Configure()
        {
            CreateMap<CustomerSource, CustomerTarget>();
        }
    }

    public sealed class OrderMappingProfile : MappingProfile
    {
        protected override void Configure()
        {
            CreateMap<OrderSource, OrderTarget>();
        }
    }

    private sealed class EmptyMappingProfile : MappingProfile
    {
        protected override void Configure()
        {
        }
    }

    private sealed class CustomerSource
    {
        public string? Name { get; set; }
    }

    private sealed class CustomerTarget
    {
        public string? Name { get; set; }
    }

    private sealed class OrderSource
    {
        public int Number { get; set; }
    }

    private sealed class OrderTarget
    {
        public int Number { get; set; }
    }
}
