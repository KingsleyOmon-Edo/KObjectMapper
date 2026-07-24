using KObjectMapper.Abstractions;

namespace KObjectMapper.UnitTests.SourceGeneration
{
    public class GeneratedMapperRegistryTests
    {
        // ── AC: Happy path – generated mapper is found and invoked ───────────────

        [Fact]
        public void TryMap_ReturnsTrue_WhenGeneratedMapperExists()
        {
            GeneratedMapperRegistry registry = new();

            PersonSource source = new() { Name = "Alice", Age = 30 };
            PersonTarget target = new();

            bool result = registry.TryMap(typeof(PersonSource), typeof(PersonTarget), source, target);

            result.ShouldBeTrue();
        }

        [Fact]
        public void TryMap_MapsProperties_WhenGeneratedMapperExists()
        {
            GeneratedMapperRegistry registry = new();

            PersonSource source = new() { Name = "Alice", Age = 30 };
            PersonTarget target = new();

            registry.TryMap(typeof(PersonSource), typeof(PersonTarget), source, target);

            target.Name.ShouldBe("Alice [generated]");
            target.Age.ShouldBe(30);
        }

        // ── AC: Fallback – no generated mapper present ───────────────────────────

        [Fact]
        public void TryMap_ReturnsFalse_WhenNoGeneratedMapperExists()
        {
            GeneratedMapperRegistry registry = new();

            UnmappedSource source = new() { Value = "test" };
            UnmappedTarget target = new();

            bool result = registry.TryMap(typeof(UnmappedSource), typeof(UnmappedTarget), source, target);

            result.ShouldBeFalse();
        }

        // ── AC: Caching – second call uses cached MethodInfo ────────────────────

        [Fact]
        public void TryMap_CachesResult_SecondCallProducesSameOutcome()
        {
            GeneratedMapperRegistry registry = new();

            PersonSource source1 = new() { Name = "Bob", Age = 25 };
            PersonTarget target1 = new();
            bool first = registry.TryMap(typeof(PersonSource), typeof(PersonTarget), source1, target1);

            PersonSource source2 = new() { Name = "Carol", Age = 40 };
            PersonTarget target2 = new();
            bool second = registry.TryMap(typeof(PersonSource), typeof(PersonTarget), source2, target2);

            first.ShouldBeTrue();
            second.ShouldBeTrue();
            target2.Name.ShouldBe("Carol [generated]");
            target2.Age.ShouldBe(40);
        }

        [Fact]
        public void TryMap_CachesNegativeResult_SecondCallReturnsFalseWithoutSearching()
        {
            GeneratedMapperRegistry registry = new();

            UnmappedSource source1 = new() { Value = "x" };
            UnmappedTarget target1 = new();
            bool first = registry.TryMap(typeof(UnmappedSource), typeof(UnmappedTarget), source1, target1);

            UnmappedSource source2 = new() { Value = "y" };
            UnmappedTarget target2 = new();
            bool second = registry.TryMap(typeof(UnmappedSource), typeof(UnmappedTarget), source2, target2);

            first.ShouldBeFalse();
            second.ShouldBeFalse();
        }

        // ── AC: Global flag disabled – generated mapper NOT used ─────────────────

        [Fact]
        public void Map_DoesNotUseGeneratedMapper_WhenGlobalFlagDisabled()
        {
            ServiceCollection services = new();
            services.AddKObjectMapper(options =>
                options.AddProfile<PersonProfile>());

            IServiceProvider provider = services.BuildServiceProvider();
            IObjectMapper mapper = provider.GetRequiredService<IObjectMapper>();

            PersonSource source = new() { Name = "Dave", Age = 22 };
            PersonTarget target = new();

            // UseSourceGeneration is false so the registry is never consulted;
            // reflection pipeline maps Name without the "[generated]" sentinel.
            mapper.Map<PersonSource, PersonTarget>(source, target);

            target.Name.ShouldBe("Dave");
            target.Age.ShouldBe(22);
        }

        // ── AC: Per-map flag disabled, global also disabled – generated mapper NOT used ──

        [Fact]
        public void Map_DoesNotUseGeneratedMapper_WhenBothFlagsDisabled()
        {
            ServiceCollection services = new();
            services.AddKObjectMapper(options =>
                options.AddProfile<PersonProfileNoGeneration>());

            IServiceProvider provider = services.BuildServiceProvider();
            IObjectMapper mapper = provider.GetRequiredService<IObjectMapper>();

            PersonSource source = new() { Name = "Eve", Age = 28 };
            PersonTarget target = new();

            // Neither global nor per-map flag is set; reflection pipeline is used.
            mapper.Map<PersonSource, PersonTarget>(source, target);

            target.Name.ShouldBe("Eve");
            target.Age.ShouldBe(28);
        }

        // ── AC: Per-map flag enabled overrides global false ──────────────────────

        [Fact]
        public void Map_UsesGeneratedMapper_WhenPerMapFlagEnabledAndGlobalDisabled()
        {
            ServiceCollection services = new();
            services.AddKObjectMapper(options =>
                options.AddProfile<PersonProfileWithGeneration>());

            IServiceProvider provider = services.BuildServiceProvider();
            IObjectMapper mapper = provider.GetRequiredService<IObjectMapper>();

            PersonSource source = new() { Name = "Frank", Age = 35 };
            PersonTarget target = new();

            mapper.Map<PersonSource, PersonTarget>(source, target);

            // The generated mapper sets a sentinel suffix to prove it was called.
            target.Name.ShouldBe("Frank [generated]");
            target.Age.ShouldBe(35);
        }

        // ── AC: Behavior parity ──────────────────────────────────────────────────

        [Fact]
        public void Map_ProducesSameAge_BothModes()
        {
            ServiceCollection servicesGen = new();
            servicesGen.AddKObjectMapper(options =>
                options.AddProfile<PersonProfileWithGeneration>());

            ServiceCollection servicesRefl = new();
            servicesRefl.AddKObjectMapper(options =>
                options.AddProfile<PersonProfile>());

            IObjectMapper mapperGen = servicesGen.BuildServiceProvider().GetRequiredService<IObjectMapper>();
            IObjectMapper mapperRefl = servicesRefl.BuildServiceProvider().GetRequiredService<IObjectMapper>();

            PersonSource source = new() { Name = "Grace", Age = 50 };

            PersonTarget targetGen = new();
            PersonTarget targetRefl = new();

            mapperGen.Map<PersonSource, PersonTarget>(source, targetGen);
            mapperRefl.Map<PersonSource, PersonTarget>(source, targetRefl);

            targetGen.Age.ShouldBe(targetRefl.Age);
        }

        // ── AC: Null source handling ─────────────────────────────────────────────

        [Fact]
        public void Map_ThrowsArgumentNullException_WhenSourceIsNull()
        {
            ServiceCollection services = new();
            services.AddKObjectMapper(options =>
                options
                    .EnableSourceGeneration()
                    .AddProfile<PersonProfileWithGeneration>());

            IServiceProvider provider = services.BuildServiceProvider();
            IObjectMapper mapper = provider.GetRequiredService<IObjectMapper>();

            PersonTarget target = new();

            Should.Throw<ArgumentNullException>(() =>
                mapper.Map<PersonSource, PersonTarget>(null!, target));
        }

        // ── Profiles ─────────────────────────────────────────────────────────────

        private sealed class PersonProfile : MappingProfile
        {
            protected override void Configure()
            {
                CreateMap<PersonSource, PersonTarget>();
            }
        }

        private sealed class PersonProfileWithGeneration : MappingProfile
        {
            protected override void Configure()
            {
                CreateMap<PersonSource, PersonTarget>().UseSourceGeneration();
            }
        }

        private sealed class PersonProfileNoGeneration : MappingProfile
        {
            protected override void Configure()
            {
                CreateMap<PersonSource, PersonTarget>();
            }
        }

        // ── Models ───────────────────────────────────────────────────────────────

        public sealed class PersonSource
        {
            public string? Name { get; set; }
            public int Age { get; set; }
        }

        public sealed class PersonTarget
        {
            public string? Name { get; set; }
            public int Age { get; set; }
        }

        public sealed class UnmappedSource
        {
            public string? Value { get; set; }
        }

        public sealed class UnmappedTarget
        {
            public string? Value { get; set; }
        }
    }
}

// ── Simulated generated mapper (mimics source-generator output) ───────────────
// Must be in the KObjectMapper.Generated namespace with the naming convention
// {SourceTypeName}To{TargetTypeName}Mapper so GeneratedMapperRegistry can find it.
namespace KObjectMapper.Generated
{
    using KObjectMapper.UnitTests.SourceGeneration;

    public static class PersonSourceToPersonTargetMapper
    {
        public static void Map(
            GeneratedMapperRegistryTests.PersonSource source,
            GeneratedMapperRegistryTests.PersonTarget target)
        {
            target.Name = source.Name + " [generated]";
            target.Age = source.Age;
        }
    }
}
