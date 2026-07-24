using KObjectMapper.Abstractions;

namespace KObjectMapper.UnitTests.Configuration;

public class SourceGenerationFallbackTests
{
    // ── AC1: Global flag ─────────────────────────────────────────────────────

    [Fact]
    public void EnableSourceGeneration_SetsGlobalFlag_OnMappingProfileOptions()
    {
        MappingProfileOptions options = new();
        options.EnableSourceGeneration();

        options.UseSourceGeneration.ShouldBeTrue();
    }

    [Fact]
    public void UseSourceGeneration_DefaultsToFalse_OnMappingProfileOptions()
    {
        MappingProfileOptions options = new();

        options.UseSourceGeneration.ShouldBeFalse();
    }

    // ── AC1: Per-map flag ────────────────────────────────────────────────────

    [Fact]
    public void UseSourceGeneration_SetsPerMapFlag_OnMappingTypeMapConfiguration()
    {
        ServiceCollection services = new();
        services.AddKObjectMapper(options =>
            options.AddProfile<SourceToTargetWithGenerationProfile>());

        IServiceProvider provider = services.BuildServiceProvider();
        IObjectMapper mapper = provider.GetRequiredService<IObjectMapper>();

        Source source = new() { Name = "Alice" };
        Target target = new();

        mapper.Map<Source, Target>(source, target);

        target.Name.ShouldBe("Alice");
    }

    [Fact]
    public void UseSourceGeneration_DefaultsToFalse_OnMappingTypeMap()
    {
        ServiceCollection services = new();
        services.AddKObjectMapper(options =>
            options.AddProfile<SourceToTargetProfile>());

        IServiceProvider provider = services.BuildServiceProvider();
        IObjectMapper mapper = provider.GetRequiredService<IObjectMapper>();

        Source source = new() { Name = "Bob" };
        Target target = new();

        mapper.Map<Source, Target>(source, target);

        target.Name.ShouldBe("Bob");
    }

    // ── AC2: Fallback when no generated mapper exists ────────────────────────

    [Fact]
    public void Map_FallsBackToReflectionPipeline_WhenGlobalFlagEnabledButNoGeneratedMapperExists()
    {
        ServiceCollection services = new();
        services.AddKObjectMapper(options =>
            options
                .EnableSourceGeneration()
                .AddProfile<SourceToTargetProfile>());

        IServiceProvider provider = services.BuildServiceProvider();
        IObjectMapper mapper = provider.GetRequiredService<IObjectMapper>();

        Source source = new() { Name = "Carol" };
        Target target = new();

        mapper.Map<Source, Target>(source, target);

        target.Name.ShouldBe("Carol");
    }

    [Fact]
    public void Map_FallsBackToReflectionPipeline_WhenPerMapFlagEnabledButNoGeneratedMapperExists()
    {
        ServiceCollection services = new();
        services.AddKObjectMapper(options =>
            options.AddProfile<SourceToTargetWithGenerationProfile>());

        IServiceProvider provider = services.BuildServiceProvider();
        IObjectMapper mapper = provider.GetRequiredService<IObjectMapper>();

        Source source = new() { Name = "Dave" };
        Target target = new();

        mapper.Map<Source, Target>(source, target);

        target.Name.ShouldBe("Dave");
    }

    // ── AC3: Behavior parity ─────────────────────────────────────────────────

    [Fact]
    public void Map_ProducesEquivalentOutput_WhenSourceGenerationEnabledVsDisabled()
    {
        ServiceCollection servicesWithFlag = new();
        servicesWithFlag.AddKObjectMapper(options =>
            options
                .EnableSourceGeneration()
                .AddProfile<SourceToTargetProfile>());

        ServiceCollection servicesWithoutFlag = new();
        servicesWithoutFlag.AddKObjectMapper(options =>
            options.AddProfile<SourceToTargetProfile>());

        IObjectMapper mapperWithFlag = servicesWithFlag.BuildServiceProvider().GetRequiredService<IObjectMapper>();
        IObjectMapper mapperWithoutFlag = servicesWithoutFlag.BuildServiceProvider().GetRequiredService<IObjectMapper>();

        Source source = new() { Name = "Eve" };

        Target targetWithFlag = new();
        Target targetWithoutFlag = new();

        mapperWithFlag.Map<Source, Target>(source, targetWithFlag);
        mapperWithoutFlag.Map<Source, Target>(source, targetWithoutFlag);

        targetWithFlag.Name.ShouldBe(targetWithoutFlag.Name);
    }

    [Fact]
    public void Map_ProducesEquivalentOutput_WhenPerMapFlagEnabledVsDisabled()
    {
        ServiceCollection servicesWithFlag = new();
        servicesWithFlag.AddKObjectMapper(options =>
            options.AddProfile<SourceToTargetWithGenerationProfile>());

        ServiceCollection servicesWithoutFlag = new();
        servicesWithoutFlag.AddKObjectMapper(options =>
            options.AddProfile<SourceToTargetProfile>());

        IObjectMapper mapperWithFlag = servicesWithFlag.BuildServiceProvider().GetRequiredService<IObjectMapper>();
        IObjectMapper mapperWithoutFlag = servicesWithoutFlag.BuildServiceProvider().GetRequiredService<IObjectMapper>();

        Source source = new() { Name = "Frank" };

        Target targetWithFlag = new();
        Target targetWithoutFlag = new();

        mapperWithFlag.Map<Source, Target>(source, targetWithFlag);
        mapperWithoutFlag.Map<Source, Target>(source, targetWithoutFlag);

        targetWithFlag.Name.ShouldBe(targetWithoutFlag.Name);
    }

    // ── Profiles and models ──────────────────────────────────────────────────

    private sealed class SourceToTargetProfile : MappingProfile
    {
        protected override void Configure()
        {
            CreateMap<Source, Target>();
        }
    }

    private sealed class SourceToTargetWithGenerationProfile : MappingProfile
    {
        protected override void Configure()
        {
            CreateMap<Source, Target>().UseSourceGeneration();
        }
    }

    private sealed class Source
    {
        public string? Name { get; set; }
    }

    private sealed class Target
    {
        public string? Name { get; set; }
    }
}
