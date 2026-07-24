using KObjectMapper.Abstractions;
using Shouldly;

namespace KObjectMapperTests.Configuration;

public class NullHandlingPolicyTests
{
    // ── US-03: Null-handling policy configuration ──────────────────────────

    // --- Propagate (default) ---

    [Fact]
    public void Map_WhenNullPolicyIsPropagate_SetsTargetMemberToNull()
    {
        ServiceCollection services = new();

        services.AddKObjectMapper(options =>
            options.AddProfile<PropagateNullProfile>());

        IObjectMapper mapper = services.BuildServiceProvider()
            .GetRequiredService<IObjectMapper>();

        OrderSource source = new() { CustomerName = null, Notes = "some note" };
        OrderTarget target = new() { CustomerName = "existing", Notes = "old" };

        mapper.Map(source, target);

        target.CustomerName.ShouldBeNull();
        target.Notes.ShouldBe("some note");
    }

    // --- Ignore ---

    [Fact]
    public void Map_WhenNullPolicyIsIgnore_KeepsExistingTargetValue()
    {
        ServiceCollection services = new();

        services.AddKObjectMapper(options =>
            options.AddProfile<IgnoreNullProfile>());

        IObjectMapper mapper = services.BuildServiceProvider()
            .GetRequiredService<IObjectMapper>();

        OrderSource source = new() { CustomerName = null, Notes = "new note" };
        OrderTarget target = new() { CustomerName = "existing", Notes = "old" };

        mapper.Map(source, target);

        target.CustomerName.ShouldBe("existing");
        target.Notes.ShouldBe("new note");
    }

    // --- Substitute ---

    [Fact]
    public void Map_WhenNullPolicyIsSubstituteWithGlobalDefault_UsesSubstituteValue()
    {
        ServiceCollection services = new();

        services.AddKObjectMapper(options =>
            options.AddProfile<SubstituteNullProfile>());

        IObjectMapper mapper = services.BuildServiceProvider()
            .GetRequiredService<IObjectMapper>();

        OrderSource source = new() { CustomerName = null, Notes = null };
        OrderTarget target = new() { CustomerName = "existing", Notes = "old" };

        mapper.Map(source, target);

        target.CustomerName.ShouldBe("Unknown");
        target.Notes.ShouldBe("N/A");
    }

    // --- Per-member substitution ---

    [Fact]
    public void Map_WhenPerMemberSubstituteConfigured_UsesPerMemberSubstituteValue()
    {
        ServiceCollection services = new();

        services.AddKObjectMapper(options =>
            options.AddProfile<PerMemberSubstituteProfile>());

        IObjectMapper mapper = services.BuildServiceProvider()
            .GetRequiredService<IObjectMapper>();

        OrderSource source = new() { CustomerName = null, Notes = null };
        OrderTarget target = new() { CustomerName = "existing", Notes = "old" };

        mapper.Map(source, target);

        target.CustomerName.ShouldBe("Guest");
        target.Notes.ShouldBeNull();
    }

    // --- Global policy via MappingProfileOptions ---

    [Fact]
    public void Map_WhenGlobalNullPolicyIsIgnore_KeepsExistingTargetValueForAllMaps()
    {
        ServiceCollection services = new();

        services.AddKObjectMapper(options =>
        {
            options.SetGlobalNullPolicy(NullMappingPolicy.Ignore);
            options.AddProfile<PlainProfile>();
        });

        IObjectMapper mapper = services.BuildServiceProvider()
            .GetRequiredService<IObjectMapper>();

        OrderSource source = new() { CustomerName = null, Notes = "new" };
        OrderTarget target = new() { CustomerName = "existing", Notes = "old" };

        mapper.Map(source, target);

        target.CustomerName.ShouldBe("existing");
        target.Notes.ShouldBe("new");
    }

    // --- Collection mapping with null policy ---

    [Fact]
    public void Map_WhenNullPolicyIsIgnoreAndCollectionMapped_KeepsExistingCollectionItems()
    {
        ServiceCollection services = new();

        services.AddKObjectMapper(options =>
            options.AddProfile<IgnoreNullProfile>());

        IObjectMapper mapper = services.BuildServiceProvider()
            .GetRequiredService<IObjectMapper>();

        OrderSource source = new() { CustomerName = null, Notes = "updated" };
        OrderTarget target = new() { CustomerName = "Alice", Notes = "old" };

        mapper.Map(source, target);

        target.CustomerName.ShouldBe("Alice");
        target.Notes.ShouldBe("updated");
    }

    // ── Profiles ──────────────────────────────────────────────────────────

    internal sealed class PropagateNullProfile : MappingProfile
    {
        protected override void Configure()
        {
            CreateMap<OrderSource, OrderTarget>()
                .WithNullPolicy(NullMappingPolicy.Propagate);
        }
    }

    internal sealed class IgnoreNullProfile : MappingProfile
    {
        protected override void Configure()
        {
            CreateMap<OrderSource, OrderTarget>()
                .WithNullPolicy(NullMappingPolicy.Ignore);
        }
    }

    internal sealed class SubstituteNullProfile : MappingProfile
    {
        protected override void Configure()
        {
            CreateMap<OrderSource, OrderTarget>()
                .WithNullPolicy(NullMappingPolicy.Substitute)
                .SubstituteNullWith(t => t.CustomerName, "Unknown")
                .SubstituteNullWith(t => t.Notes, "N/A");
        }
    }

    internal sealed class PerMemberSubstituteProfile : MappingProfile
    {
        protected override void Configure()
        {
            CreateMap<OrderSource, OrderTarget>()
                .WithNullPolicy(NullMappingPolicy.Propagate)
                .SubstituteNullWith(t => t.CustomerName, "Guest");
        }
    }

    internal sealed class PlainProfile : MappingProfile
    {
        protected override void Configure()
        {
            CreateMap<OrderSource, OrderTarget>();
        }
    }

    // ── Test models ───────────────────────────────────────────────────────

    public sealed class OrderSource
    {
        public string? CustomerName { get; set; }
        public string? Notes { get; set; }
    }

    public sealed class OrderTarget
    {
        public string? CustomerName { get; set; }
        public string? Notes { get; set; }
    }
}
