using KObjectMapper.Abstractions;

namespace KObjectMapper.UnitTests.Diagnostics;

public class MappingResultTests
{
    private sealed class Source
    {
        public string Name { get; set; } = string.Empty;
        public int Age { get; set; }
    }

    private sealed class Target
    {
        public string Name { get; set; } = string.Empty;
        public int Age { get; set; }
    }

    private static IObjectMapper BuildMapper(Action<MappingProfileOptions>? configure = null)
    {
        ServiceCollection services = new();
        if (configure is not null)
            services.AddKObjectMapper(configure);
        else
            services.AddKObjectMapper();
        return services.BuildServiceProvider().GetRequiredService<IObjectMapper>();
    }

    [Fact]
    public void TryMap_ReturnsSuccess_WhenMappingSucceeds()
    {
        IObjectMapper mapper = BuildMapper();
        Source source = new() { Name = "Alice", Age = 30 };
        Target target = new();

        MappingResult result = mapper.TryMap(source, target);

        result.IsSuccess.ShouldBeTrue();
        result.Errors.ShouldBeEmpty();
    }

    [Fact]
    public void TryMap_MapsProperties_WhenMappingSucceeds()
    {
        IObjectMapper mapper = BuildMapper();
        Source source = new() { Name = "Bob", Age = 25 };
        Target target = new();

        mapper.TryMap(source, target);

        target.Name.ShouldBe("Bob");
        target.Age.ShouldBe(25);
    }

    [Fact]
    public void TryMap_Generic_ReturnsSuccess_WhenMappingSucceeds()
    {
        IObjectMapper mapper = BuildMapper();
        Source source = new() { Name = "Carol", Age = 40 };
        Target target = new();

        MappingResult result = mapper.TryMap<Source, Target>(source, target);

        result.IsSuccess.ShouldBeTrue();
        result.Errors.ShouldBeEmpty();
    }

    [Fact]
    public void TryMap_ReturnsFailure_WhenSourceIsNull()
    {
        IObjectMapper mapper = BuildMapper();
        Target target = new();

        MappingResult result = mapper.TryMap(null!, target);

        result.IsSuccess.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
    }

    [Fact]
    public void TryMap_ReturnsFailure_WhenNoProfileRegistered_InStrictMode()
    {
        ServiceCollection services = new();
        services.AddKObjectMapper(options => options.EnableStrictMode());
        IObjectMapper mapper = services.BuildServiceProvider().GetRequiredService<IObjectMapper>();

        Source source = new() { Name = "Dave", Age = 20 };
        Target target = new();

        MappingResult result = mapper.TryMap<Source, Target>(source, target);

        result.IsSuccess.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
    }

    [Fact]
    public void TryMap_FailureResult_ContainsMemberPath()
    {
        IObjectMapper mapper = BuildMapper();

        MappingResult result = mapper.TryMap(null!, new Target());

        result.Errors[0].MemberPath.ShouldNotBeNull();
    }

    [Fact]
    public void TryMap_FailureResult_ContainsSourceAndTargetTypes()
    {
        ServiceCollection services = new();
        services.AddKObjectMapper(options => options.EnableStrictMode());
        IObjectMapper mapper = services.BuildServiceProvider().GetRequiredService<IObjectMapper>();

        MappingResult result = mapper.TryMap<Source, Target>(new Source(), new Target());

        result.Errors[0].SourceType.ShouldBe(typeof(Source));
        result.Errors[0].TargetType.ShouldBe(typeof(Target));
    }

    [Fact]
    public void TryMap_FailureResult_ContainsFailureReason()
    {
        IObjectMapper mapper = BuildMapper();

        MappingResult result = mapper.TryMap(null!, new Target());

        result.Errors[0].Reason.ShouldNotBeNullOrEmpty();
    }

    [Fact]
    public void TryMap_InvokesOnMappingError_WhenMappingFails()
    {
        MappingError? capturedError = null;
        ServiceCollection services = new();
        services.AddKObjectMapper(options =>
            options.EnableStrictMode()
                   .WithOnMappingError(e => capturedError = e));
        IObjectMapper mapper = services.BuildServiceProvider().GetRequiredService<IObjectMapper>();

        mapper.TryMap<Source, Target>(new Source(), new Target());

        capturedError.ShouldNotBeNull();
    }

    [Fact]
    public void TryMap_InvokesOnMappingCompleted_WhenMappingSucceeds()
    {
        MappingResult? capturedResult = null;
        ServiceCollection services = new();
        services.AddKObjectMapper(options =>
            options.WithOnMappingCompleted(r => capturedResult = r));
        IObjectMapper mapper = services.BuildServiceProvider().GetRequiredService<IObjectMapper>();

        mapper.TryMap(new Source(), new Target());

        capturedResult.ShouldNotBeNull();
        capturedResult!.IsSuccess.ShouldBeTrue();
    }

    [Fact]
    public void TryMap_DoesNotInvokeOnMappingError_WhenMappingSucceeds()
    {
        bool errorInvoked = false;
        ServiceCollection services = new();
        services.AddKObjectMapper(options =>
            options.WithOnMappingError(_ => errorInvoked = true));
        IObjectMapper mapper = services.BuildServiceProvider().GetRequiredService<IObjectMapper>();

        mapper.TryMap(new Source(), new Target());

        errorInvoked.ShouldBeFalse();
    }

    [Fact]
    public void TryMap_ReturnsSuccess_WhenSourceAndTargetAreIdentical()
    {
        IObjectMapper mapper = BuildMapper();
        Source obj = new() { Name = "Eve", Age = 35 };

        MappingResult result = mapper.TryMap(obj, obj);

        result.IsSuccess.ShouldBeTrue();
    }

    [Fact]
    public void TryMap_ReturnsSuccess_ForCollectionMapping()
    {
        IObjectMapper mapper = BuildMapper();
        Source source = new() { Name = "Frank", Age = 22 };
        Target target = new();

        MappingResult result = mapper.TryMap<Source, Target>(source, target);

        result.IsSuccess.ShouldBeTrue();
        target.Name.ShouldBe("Frank");
    }
}
