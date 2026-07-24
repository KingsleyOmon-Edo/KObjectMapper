using System.Reflection;
using KObjectMapper.Abstractions;
using KObjectMapper.Security;

namespace KObjectMapper.UnitTests.Security;

public class SensitiveDataGuardTests
{
    // ── Attribute-based exclusion (AC1) ──────────────────────────────────────

    [Fact]
    public void Map_WithSensitiveAttribute_OnProperty_ExcludesProperty()
    {
        ServiceCollection services = new();
        services.AddKObjectMapper(options => options.AddProfile<UserProfile>());
        IServiceProvider provider = services.BuildServiceProvider();
        IObjectMapper mapper = provider.GetRequiredService<IObjectMapper>();

        UserSource source = new() { Name = "Alice", Password = "secret" };
        UserTarget target = new();

        mapper.Map(source, target);

        target.Name.ShouldBe("Alice");
        target.Password.ShouldBeNull();
    }

    [Fact]
    public void Map_WithSensitiveAttribute_OnClass_ExcludesAllProperties()
    {
        ServiceCollection services = new();
        services.AddKObjectMapper(options => options.AddProfile<SensitiveClassProfile>());
        IServiceProvider provider = services.BuildServiceProvider();
        IObjectMapper mapper = provider.GetRequiredService<IObjectMapper>();

        SensitiveSource source = new() { Name = "Alice", Email = "alice@example.com" };
        SensitiveTarget target = new();

        mapper.Map(source, target);

        target.Name.ShouldBeNull();
        target.Email.ShouldBeNull();
    }

    [Fact]
    public void Map_WithoutSensitiveAttribute_MapsNormally()
    {
        ServiceCollection services = new();
        services.AddKObjectMapper(options => options.AddProfile<PlainProfile>());
        IServiceProvider provider = services.BuildServiceProvider();
        IObjectMapper mapper = provider.GetRequiredService<IObjectMapper>();

        PlainSource source = new() { Name = "Alice", Age = 30 };
        PlainTarget target = new();

        mapper.Map(source, target);

        target.Name.ShouldBe("Alice");
        target.Age.ShouldBe(30);
    }

    // ── Configuration-based exclusion (AC1) ──────────────────────────────────

    [Fact]
    public void Map_WithSensitivePolicy_ExcludeMarked_OnlyExcludesMarkedMembers()
    {
        ServiceCollection services = new();
        services.AddKObjectMapper(options => options
            .SetSensitivePolicy(SensitiveMappingPolicy.ExcludeMarked)
            .AddProfile<UserProfile>());
        IServiceProvider provider = services.BuildServiceProvider();
        IObjectMapper mapper = provider.GetRequiredService<IObjectMapper>();

        UserSource source = new() { Name = "Bob", Password = "pass123" };
        UserTarget target = new();

        mapper.Map(source, target);

        target.Name.ShouldBe("Bob");
        target.Password.ShouldBeNull();
    }

    // ── Default-deny mode (AC2) ───────────────────────────────────────────────

    [Fact]
    public void Map_WithDefaultDenyPolicy_ExcludesAllMembersNotAllowed()
    {
        ServiceCollection services = new();
        services.AddKObjectMapper(options => options.AddProfile<DefaultDenyNoAllowProfile>());
        IServiceProvider provider = services.BuildServiceProvider();
        IObjectMapper mapper = provider.GetRequiredService<IObjectMapper>();

        PlainSource source = new() { Name = "Alice", Age = 30 };
        PlainTarget target = new();

        mapper.Map(source, target);

        target.Name.ShouldBeNull();
        target.Age.ShouldBe(0);
    }

    [Fact]
    public void Map_WithDefaultDenyPolicy_AllowedMembersAreMapped()
    {
        ServiceCollection services = new();
        services.AddKObjectMapper(options => options.AddProfile<DefaultDenyAllowNameProfile>());
        IServiceProvider provider = services.BuildServiceProvider();
        IObjectMapper mapper = provider.GetRequiredService<IObjectMapper>();

        PlainSource source = new() { Name = "Alice", Age = 30 };
        PlainTarget target = new();

        mapper.Map(source, target);

        target.Name.ShouldBe("Alice");
    }

    [Fact]
    public void Map_WithDefaultDenyPolicy_NonAllowedMembersAreExcluded()
    {
        ServiceCollection services = new();
        services.AddKObjectMapper(options => options.AddProfile<DefaultDenyAllowNameProfile>());
        IServiceProvider provider = services.BuildServiceProvider();
        IObjectMapper mapper = provider.GetRequiredService<IObjectMapper>();

        PlainSource source = new() { Name = "Alice", Age = 30 };
        PlainTarget target = new();

        mapper.Map(source, target);

        target.Age.ShouldBe(0);
    }

    [Fact]
    public void Map_WithDefaultDenyPolicy_MultipleAllowedMembers()
    {
        ServiceCollection services = new();
        services.AddKObjectMapper(options => options.AddProfile<DefaultDenyAllowBothProfile>());
        IServiceProvider provider = services.BuildServiceProvider();
        IObjectMapper mapper = provider.GetRequiredService<IObjectMapper>();

        PlainSource source = new() { Name = "Alice", Age = 30 };
        PlainTarget target = new();

        mapper.Map(source, target);

        target.Name.ShouldBe("Alice");
        target.Age.ShouldBe(30);
    }

    // ── Verification (AC3) ────────────────────────────────────────────────────

    [Fact]
    public void Map_SensitiveProperty_IsNeverCopied_EvenWhenSourceHasValue()
    {
        ServiceCollection services = new();
        services.AddKObjectMapper(options => options.AddProfile<UserProfile>());
        IServiceProvider provider = services.BuildServiceProvider();
        IObjectMapper mapper = provider.GetRequiredService<IObjectMapper>();

        UserSource source = new() { Name = "Alice", Password = "super-secret" };
        UserTarget target = new() { Password = "original" };

        mapper.Map(source, target);

        target.Password.ShouldBe("original");
    }

    [Fact]
    public void Map_SensitiveProperty_DoesNotThrow_JustSkips()
    {
        ServiceCollection services = new();
        services.AddKObjectMapper(options => options.AddProfile<UserProfile>());
        IServiceProvider provider = services.BuildServiceProvider();
        IObjectMapper mapper = provider.GetRequiredService<IObjectMapper>();

        UserSource source = new() { Name = "Alice", Password = "secret" };
        UserTarget target = new();

        Should.NotThrow(() => mapper.Map(source, target));
    }

    [Fact]
    public void Map_NonSensitiveProperties_AreMapped_WhenSensitiveOnesAreExcluded()
    {
        ServiceCollection services = new();
        services.AddKObjectMapper(options => options.AddProfile<UserProfile>());
        IServiceProvider provider = services.BuildServiceProvider();
        IObjectMapper mapper = provider.GetRequiredService<IObjectMapper>();

        UserSource source = new() { Name = "Alice", Password = "secret" };
        UserTarget target = new();

        mapper.Map(source, target);

        target.Name.ShouldBe("Alice");
        target.Password.ShouldBeNull();
    }

    // ── Edge cases ────────────────────────────────────────────────────────────

    [Fact]
    public void SensitiveDataGuard_IsExcluded_ReturnsFalse_ForNonSensitiveProperty()
    {
        PropertyInfo property = typeof(PlainSource).GetProperty(nameof(PlainSource.Name))!;

        bool result = SensitiveDataGuard.IsExcluded(property, typeof(PlainSource), SensitiveMappingPolicy.ExcludeMarked, new HashSet<string>());

        result.ShouldBeFalse();
    }

    [Fact]
    public void SensitiveDataGuard_IsExcluded_ReturnsTrue_ForSensitiveProperty()
    {
        PropertyInfo property = typeof(UserSource).GetProperty(nameof(UserSource.Password))!;

        bool result = SensitiveDataGuard.IsExcluded(property, typeof(UserSource), SensitiveMappingPolicy.ExcludeMarked, new HashSet<string>());

        result.ShouldBeTrue();
    }

    [Fact]
    public void Map_WithSensitiveAttribute_OnInheritedProperty_IsExcluded()
    {
        ServiceCollection services = new();
        services.AddKObjectMapper(options => options.AddProfile<InheritedProfile>());
        IServiceProvider provider = services.BuildServiceProvider();
        IObjectMapper mapper = provider.GetRequiredService<IObjectMapper>();

        DerivedSource source = new() { Name = "Alice", Password = "secret" };
        DerivedTarget target = new();

        mapper.Map(source, target);

        target.Name.ShouldBe("Alice");
        target.Password.ShouldBeNull();
    }

    // ── Profiles ──────────────────────────────────────────────────────────────

    private sealed class UserProfile : MappingProfile
    {
        protected override void Configure()
        {
            CreateMap<UserSource, UserTarget>();
        }
    }

    private sealed class SensitiveClassProfile : MappingProfile
    {
        protected override void Configure()
        {
            CreateMap<SensitiveSource, SensitiveTarget>();
        }
    }

    private sealed class PlainProfile : MappingProfile
    {
        protected override void Configure()
        {
            CreateMap<PlainSource, PlainTarget>();
        }
    }

    private sealed class DefaultDenyNoAllowProfile : MappingProfile
    {
        protected override void Configure()
        {
            CreateMap<PlainSource, PlainTarget>()
                .SetSensitivePolicy(SensitiveMappingPolicy.DefaultDeny);
        }
    }

    private sealed class DefaultDenyAllowNameProfile : MappingProfile
    {
        protected override void Configure()
        {
            CreateMap<PlainSource, PlainTarget>()
                .SetSensitivePolicy(SensitiveMappingPolicy.DefaultDeny)
                .AllowMember(t => t.Name);
        }
    }

    private sealed class DefaultDenyAllowBothProfile : MappingProfile
    {
        protected override void Configure()
        {
            CreateMap<PlainSource, PlainTarget>()
                .SetSensitivePolicy(SensitiveMappingPolicy.DefaultDeny)
                .AllowMember(t => t.Name)
                .AllowMember(t => t.Age);
        }
    }

    private sealed class InheritedProfile : MappingProfile
    {
        protected override void Configure()
        {
            CreateMap<DerivedSource, DerivedTarget>();
        }
    }

    // ── Models ────────────────────────────────────────────────────────────────

    private sealed class UserSource
    {
        public string? Name { get; set; }
        [Sensitive]
        public string? Password { get; set; }
    }

    private sealed class UserTarget
    {
        public string? Name { get; set; }
        public string? Password { get; set; }
    }

    [Sensitive]
    private sealed class SensitiveSource
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
    }

    private sealed class SensitiveTarget
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
    }

    private sealed class PlainSource
    {
        public string? Name { get; set; }
        public int Age { get; set; }
    }

    private sealed class PlainTarget
    {
        public string? Name { get; set; }
        public int Age { get; set; }
    }

    private class BaseSource
    {
        public string? Name { get; set; }
        [Sensitive]
        public string? Password { get; set; }
    }

    private sealed class DerivedSource : BaseSource { }

    private class BaseTarget
    {
        public string? Name { get; set; }
        public string? Password { get; set; }
    }

    private sealed class DerivedTarget : BaseTarget { }
}
