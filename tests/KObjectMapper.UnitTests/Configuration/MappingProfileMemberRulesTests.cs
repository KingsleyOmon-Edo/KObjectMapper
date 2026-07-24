using KObjectMapper.Abstractions;
using Shouldly;

namespace KObjectMapper.UnitTests.Configuration;

public class MappingProfileMemberRulesTests
{
    [Fact]
    public void Mapper_WhenProfileDefinesDifferentSourceAndTargetNames_MapsConfiguredMember()
    {
        ServiceCollection services = new();

        services.AddKObjectMapper(options => options.AddProfile<PersonProfile>());

        IServiceProvider provider = services.BuildServiceProvider();
        IObjectMapper mapper = provider.GetRequiredService<IObjectMapper>();

        PersonSource source = new()
        {
            GivenName = "Ada",
            Surname = "Lovelace",
            Age = 36
        };

        PersonTarget target = new()
        {
            FirstName = string.Empty,
            LastName = string.Empty,
            Age = 0
        };

        mapper.Map(source, target);

        target.FirstName.ShouldBe("Ada");
        target.LastName.ShouldBe("Lovelace");
        target.Age.ShouldBe(36);
    }

    [Fact]
    public void Mapper_WhenProfileIgnoresTargetMember_KeepsExistingTargetValue()
    {
        ServiceCollection services = new();

        services.AddKObjectMapper(options => options.AddProfile<PersonIgnoreProfile>());

        IServiceProvider provider = services.BuildServiceProvider();
        IObjectMapper mapper = provider.GetRequiredService<IObjectMapper>();

        PersonSource source = new()
        {
            GivenName = "Ada",
            Surname = "Lovelace",
            Age = 36
        };

        PersonTarget target = new()
        {
            FirstName = string.Empty,
            LastName = "Existing",
            Age = 0
        };

        mapper.Map(source, target);

        target.FirstName.ShouldBe("Ada");
        target.LastName.ShouldBe("Existing");
        target.Age.ShouldBe(36);
    }

    [Fact]
    public void AddKObjectMapper_WhenRequiredTargetMemberIsUnmapped_ThrowsValidationException()
    {
        Should.Throw<MappingProfileValidationException>(() =>
            new ServiceCollection().AddKObjectMapper(options => options.AddProfile<IncompletePersonProfile>()));
    }

    internal sealed class PersonProfile : MappingProfile
    {
        protected override void Configure()
        {
            CreateMap<PersonSource, PersonTarget>()
                .ForMember(source => source.GivenName, target => target.FirstName)
                .ForMember(source => source.Surname, target => target.LastName);
        }
    }

    internal sealed class PersonIgnoreProfile : MappingProfile
    {
        protected override void Configure()
        {
            CreateMap<PersonSource, PersonTarget>()
                .ForMember(source => source.GivenName, target => target.FirstName)
                .ForMember(source => source.Surname, target => target.LastName)
                .Ignore(target => target.LastName);
        }
    }

    internal sealed class IncompletePersonProfile : MappingProfile
    {
        protected override void Configure()
        {
            CreateMap<PersonSource, PersonTarget>()
                .ForMember(source => source.GivenName, target => target.FirstName);
        }
    }

    public sealed class PersonSource
    {
        public string? GivenName { get; set; }

        public string? Surname { get; set; }

        public int Age { get; set; }
    }

    public sealed class PersonTarget
    {
        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public int Age { get; set; }
    }
}
