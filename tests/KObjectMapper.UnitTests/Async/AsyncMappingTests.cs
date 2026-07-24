using KObjectMapper.Abstractions;

namespace KObjectMapper.UnitTests.Async;

public class AsyncMappingTests
{
    private sealed class Source
    {
        public string Name { get; set; } = string.Empty;
        public int Value { get; set; }
    }

    private sealed class Target
    {
        public string Name { get; set; } = string.Empty;
        public int Value { get; set; }
    }

    private static IAsyncObjectMapper BuildMapper(Action<MappingProfileOptions>? configure = null)
    {
        ServiceCollection services = new();
        services.AddKObjectMapper(opts =>
        {
            configure?.Invoke(opts);
        });
        ServiceProvider sp = services.BuildServiceProvider();
        return sp.GetRequiredService<IAsyncObjectMapper>();
    }

    // AC1: Happy path

    [Fact]
    public async Task MapAsync_MapsProperties_WhenCancellationNotRequested()
    {
        IAsyncObjectMapper mapper = BuildMapper();
        Source source = new() { Name = "Alice", Value = 42 };
        Target target = new();

        await mapper.MapAsync(source, target);

        target.Name.ShouldBe("Alice");
        target.Value.ShouldBe(42);
    }

    [Fact]
    public async Task MapAsync_Generic_MapsProperties_WhenCancellationNotRequested()
    {
        IAsyncObjectMapper mapper = BuildMapper();
        Source source = new() { Name = "Bob", Value = 7 };
        Target target = new();

        await mapper.MapAsync<Source, Target>(source, target);

        target.Name.ShouldBe("Bob");
        target.Value.ShouldBe(7);
    }

    [Fact]
    public async Task MapAsync_CompletesSuccessfully_WithDefaultCancellationToken()
    {
        IAsyncObjectMapper mapper = BuildMapper();
        Source source = new() { Name = "Carol", Value = 1 };
        Target target = new();

        await mapper.MapAsync(source, target, default);

        target.Name.ShouldBe("Carol");
    }

    // AC3: Cancellation

    [Fact]
    public async Task MapAsync_ThrowsOperationCanceledException_WhenTokenAlreadyCancelled()
    {
        IAsyncObjectMapper mapper = BuildMapper();
        using CancellationTokenSource cts = new();
        cts.Cancel();

        await Should.ThrowAsync<OperationCanceledException>(
            () => mapper.MapAsync(new Source(), new Target(), cts.Token));
    }

    [Fact]
    public async Task MapAsync_Generic_ThrowsOperationCanceledException_WhenTokenAlreadyCancelled()
    {
        IAsyncObjectMapper mapper = BuildMapper();
        using CancellationTokenSource cts = new();
        cts.Cancel();

        await Should.ThrowAsync<OperationCanceledException>(
            () => mapper.MapAsync<Source, Target>(new Source(), new Target(), cts.Token));
    }

    [Fact]
    public async Task TryMapAsync_ReturnsFailure_WhenTokenAlreadyCancelled()
    {
        IAsyncObjectMapper mapper = BuildMapper();
        using CancellationTokenSource cts = new();
        cts.Cancel();

        MappingResult result = await mapper.TryMapAsync(new Source(), new Target(), cts.Token);

        result.IsSuccess.ShouldBeFalse();
    }

    [Fact]
    public async Task MapAsync_ThrowsOperationCanceledException_WhenTokenCancelledDuringAsyncConverter()
    {
        using CancellationTokenSource cts = new();

        ServiceCollection services = new();
        services.AddKObjectMapper(opts =>
        {
            opts.AddAsyncConverter<Source, Target>(new CancellingConverter(cts));
        });
        ServiceProvider sp = services.BuildServiceProvider();
        IAsyncObjectMapper mapper = sp.GetRequiredService<IAsyncObjectMapper>();

        await Should.ThrowAsync<OperationCanceledException>(
            () => mapper.MapAsync<Source, Target>(new Source(), new Target(), cts.Token));
    }

    // AC2: Async converters

    [Fact]
    public async Task MapAsync_UsesAsyncConverter_WhenRegistered()
    {
        ServiceCollection services = new();
        services.AddKObjectMapper(opts =>
        {
            opts.AddAsyncConverter<Source, Target>(new NamePrefixConverter("ASYNC_"));
        });
        ServiceProvider sp = services.BuildServiceProvider();
        IAsyncObjectMapper mapper = sp.GetRequiredService<IAsyncObjectMapper>();

        Source source = new() { Name = "Dave", Value = 5 };
        Target target = new();

        await mapper.MapAsync<Source, Target>(source, target);

        target.Name.ShouldBe("ASYNC_Dave");
    }

    [Fact]
    public async Task MapAsync_AsyncConverter_CanBeAsync_WithAwait()
    {
        ServiceCollection services = new();
        services.AddKObjectMapper(opts =>
        {
            opts.AddAsyncConverter<Source, Target>(new DelayedConverter());
        });
        ServiceProvider sp = services.BuildServiceProvider();
        IAsyncObjectMapper mapper = sp.GetRequiredService<IAsyncObjectMapper>();

        Source source = new() { Name = "Eve", Value = 9 };
        Target target = new();

        await mapper.MapAsync<Source, Target>(source, target);

        target.Name.ShouldBe("Eve");
        target.Value.ShouldBe(9);
    }

    [Fact]
    public async Task MapAsync_AsyncConverter_ReceivesCancellationToken()
    {
        TokenCapturingConverter capturingConverter = new();
        ServiceCollection services = new();
        services.AddKObjectMapper(opts =>
        {
            opts.AddAsyncConverter<Source, Target>(capturingConverter);
        });
        ServiceProvider sp = services.BuildServiceProvider();
        IAsyncObjectMapper mapper = sp.GetRequiredService<IAsyncObjectMapper>();

        using CancellationTokenSource cts = new();
        await mapper.MapAsync<Source, Target>(new Source(), new Target(), cts.Token);

        capturingConverter.ReceivedToken.ShouldBe(cts.Token);
    }

    // TryMapAsync

    [Fact]
    public async Task TryMapAsync_ReturnsSuccess_WhenMappingSucceeds()
    {
        IAsyncObjectMapper mapper = BuildMapper();
        Source source = new() { Name = "Frank", Value = 3 };
        Target target = new();

        MappingResult result = await mapper.TryMapAsync(source, target);

        result.IsSuccess.ShouldBeTrue();
    }

    [Fact]
    public async Task TryMapAsync_ReturnsFailure_WhenSourceIsNull()
    {
        IAsyncObjectMapper mapper = BuildMapper();

        MappingResult result = await mapper.TryMapAsync(null!, new Target());

        result.IsSuccess.ShouldBeFalse();
    }

    // Edge cases

    [Fact]
    public async Task MapAsync_WithNullSource_ThrowsOrReturnsFailure()
    {
        IAsyncObjectMapper mapper = BuildMapper();

        await Should.ThrowAsync<Exception>(() => mapper.MapAsync(null!, new Target()));
    }

    [Fact]
    public async Task MapAsync_MultipleAsyncConverters_EachInvokedForCorrectType()
    {
        NamePrefixConverter converterA = new("A_");
        NamePrefixConverter2 converterB = new("B_");

        ServiceCollection services = new();
        services.AddKObjectMapper(opts =>
        {
            opts.AddAsyncConverter<Source, Target>(converterA);
            opts.AddAsyncConverter<Source, AltTarget>(converterB);
        });
        ServiceProvider sp = services.BuildServiceProvider();
        IAsyncObjectMapper mapper = sp.GetRequiredService<IAsyncObjectMapper>();

        Source source = new() { Name = "Grace", Value = 0 };
        Target target = new();
        AltTarget altTarget = new();

        await mapper.MapAsync<Source, Target>(source, target);
        await mapper.MapAsync<Source, AltTarget>(source, altTarget);

        target.Name.ShouldBe("A_Grace");
        altTarget.Name.ShouldBe("B_Grace");
    }

    // Helper types

    private sealed class AltTarget
    {
        public string Name { get; set; } = string.Empty;
        public int Value { get; set; }
    }

    private sealed class NamePrefixConverter(string prefix) : IAsyncTypeConverter<Source, Target>
    {
        public Task<Target> ConvertAsync(Source source, CancellationToken cancellationToken = default)
            => Task.FromResult(new Target { Name = prefix + source.Name, Value = source.Value });
    }

    private sealed class NamePrefixConverter2(string prefix) : IAsyncTypeConverter<Source, AltTarget>
    {
        public Task<AltTarget> ConvertAsync(Source source, CancellationToken cancellationToken = default)
            => Task.FromResult(new AltTarget { Name = prefix + source.Name, Value = source.Value });
    }

    private sealed class DelayedConverter : IAsyncTypeConverter<Source, Target>
    {
        public async Task<Target> ConvertAsync(Source source, CancellationToken cancellationToken = default)
        {
            await Task.Delay(10, cancellationToken).ConfigureAwait(false);
            return new Target { Name = source.Name, Value = source.Value };
        }
    }

    private sealed class CancellingConverter(CancellationTokenSource cts) : IAsyncTypeConverter<Source, Target>
    {
        public async Task<Target> ConvertAsync(Source source, CancellationToken cancellationToken = default)
        {
            await cts.CancelAsync().ConfigureAwait(false);
            cancellationToken.ThrowIfCancellationRequested();
            return new Target();
        }
    }

    private sealed class TokenCapturingConverter : IAsyncTypeConverter<Source, Target>
    {
        public CancellationToken ReceivedToken { get; private set; }

        public Task<Target> ConvertAsync(Source source, CancellationToken cancellationToken = default)
        {
            ReceivedToken = cancellationToken;
            return Task.FromResult(new Target { Name = source.Name, Value = source.Value });
        }
    }
}
