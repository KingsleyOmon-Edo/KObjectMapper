using KObjectMapper.Abstractions;

namespace KObjectMapperTests.Configuration;

public class StrictMappingModeTests
{
    // ── Strict mode: runtime enforcement ────────────────────────────────────

    [Fact]
    public void Map_ThrowsInvalidOperationException_WhenStrictModeEnabledAndNoTypeMapRegistered()
    {
        ServiceCollection services = new();
        services.AddKObjectMapper(options =>
            options
                .EnableStrictMode()
                .AddProfile<SourceToTargetProfile>());

        IServiceProvider provider = services.BuildServiceProvider();
        IObjectMapper mapper = provider.GetRequiredService<IObjectMapper>();

        Source source = new() { Name = "Alice" };
        Target target = new();

        Should.Throw<InvalidOperationException>(() =>
            mapper.Map<Source, Unregistered>(source, new Unregistered()));
    }

    [Fact]
    public void Map_Succeeds_WhenStrictModeEnabledAndTypeMapIsRegistered()
    {
        ServiceCollection services = new();
        services.AddKObjectMapper(options =>
            options
                .EnableStrictMode()
                .AddProfile<SourceToTargetProfile>());

        IServiceProvider provider = services.BuildServiceProvider();
        IObjectMapper mapper = provider.GetRequiredService<IObjectMapper>();

        Source source = new() { Name = "Bob" };
        Target target = new();

        mapper.Map<Source, Target>(source, target);

        target.Name.ShouldBe("Bob");
    }

    [Fact]
    public void Map_DoesNotThrow_WhenStrictModeDisabledAndNoTypeMapRegistered()
    {
        ServiceCollection services = new();
        services.AddKObjectMapper(options =>
            options.AddProfile<SourceToTargetProfile>());

        IServiceProvider provider = services.BuildServiceProvider();
        IObjectMapper mapper = provider.GetRequiredService<IObjectMapper>();

        Source source = new() { Name = "Carol" };
        Unregistered target = new();

        Should.NotThrow(() => mapper.Map<Source, Unregistered>(source, target));
    }

    // ── Startup validation: ambiguous constructor binding ───────────────────

    [Fact]
    public void AddKObjectMapper_ThrowsMappingProfileValidationException_WhenTargetHasNoParameterlessConstructorAndNoConstructorMapping()
    {
        ServiceCollection services = new();

        Should.Throw<MappingProfileValidationException>(() =>
            services.AddKObjectMapper(options =>
                options.AddProfile<SourceToNoDefaultCtorTargetProfile>()));
    }

    [Fact]
    public void AddKObjectMapper_ValidationErrors_ContainAmbiguousConstructorMessage_WhenTargetLacksParameterlessConstructor()
    {
        ServiceCollection services = new();

        MappingProfileValidationException ex = Should.Throw<MappingProfileValidationException>(() =>
            services.AddKObjectMapper(options =>
                options.AddProfile<SourceToNoDefaultCtorTargetProfile>()));

        ex.Errors.ShouldContain(e => e.Contains("constructor") && e.Contains(nameof(NoDefaultCtorTarget)));
    }

    // ── Structured validation output ─────────────────────────────────────────

    [Fact]
    public void MappingProfileValidationException_ExposesAllErrors_AsStructuredCollection()
    {
        ServiceCollection services = new();

        MappingProfileValidationException ex = Should.Throw<MappingProfileValidationException>(() =>
            services.AddKObjectMapper(options =>
                options.AddProfile<MultiErrorProfile>()));

        ex.Errors.Count.ShouldBeGreaterThan(0);
        ex.Message.ShouldNotBeNullOrWhiteSpace();
    }

    // ── Profiles and models ──────────────────────────────────────────────────

    private sealed class SourceToTargetProfile : MappingProfile
    {
        protected override void Configure()
        {
            CreateMap<Source, Target>();
        }
    }

    private sealed class SourceToNoDefaultCtorTargetProfile : MappingProfile
    {
        protected override void Configure()
        {
            CreateMap<Source, NoDefaultCtorTarget>();
        }
    }

    private sealed class MultiErrorProfile : MappingProfile
    {
        protected override void Configure()
        {
            CreateMap<Source, MismatchedTarget>();
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

    private sealed class Unregistered
    {
        public string? Name { get; set; }
    }

    private sealed class NoDefaultCtorTarget
    {
        public NoDefaultCtorTarget(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
    }

    private sealed class MismatchedTarget
    {
        public string? Name { get; set; }
        public int UnmappedRequired { get; set; }
    }
}
