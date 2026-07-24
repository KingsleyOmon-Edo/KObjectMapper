using KObjectMapper.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace KObjectMapperTests.Configuration;

public class TypeConverterTests
{
    // ── Converter interface and registration ─────────────────────────────────

    [Fact]
    public void GlobalConverter_IsInvoked_WhenMappingRegisteredTypePair()
    {
        ServiceCollection services = new();
        services.AddKObjectMapper(options =>
            options
                .AddProfile<OrderProfile>()
                .AddConverter(new StringToIntConverter()));

        IServiceProvider provider = services.BuildServiceProvider();
        IObjectMapper mapper = provider.GetRequiredService<IObjectMapper>();

        OrderSource source = new() { Quantity = "42" };
        OrderTarget target = new();

        mapper.Map<OrderSource, OrderTarget>(source, target);

        target.Quantity.ShouldBe(42);
    }

    [Fact]
    public void PerMapConverter_IsInvoked_WhenMappingRegisteredTypePair()
    {
        ServiceCollection services = new();
        services.AddKObjectMapper(options =>
            options.AddProfile<OrderWithPerMapConverterProfile>());

        IServiceProvider provider = services.BuildServiceProvider();
        IObjectMapper mapper = provider.GetRequiredService<IObjectMapper>();

        OrderSource source = new() { Quantity = "7" };
        OrderTarget target = new();

        mapper.Map<OrderSource, OrderTarget>(source, target);

        target.Quantity.ShouldBe(7);
    }

    // ── Built-in converters: primitives ──────────────────────────────────────

    [Theory]
    [InlineData("123", 123)]
    [InlineData("0", 0)]
    [InlineData("-5", -5)]
    public void BuiltInStringToInt32Converter_ConvertsCorrectly(string input, int expected)
    {
        ITypeConverter<string, int> converter = TypeConverters.StringToInt32;
        converter.Convert(input).ShouldBe(expected);
    }

    [Theory]
    [InlineData("3.14")]
    [InlineData("0.0")]
    public void BuiltInStringToDoubleConverter_ConvertsCorrectly(string input)
    {
        ITypeConverter<string, double> converter = TypeConverters.StringToDouble;
        double result = converter.Convert(input);
        result.ShouldBe(double.Parse(input, System.Globalization.CultureInfo.InvariantCulture));
    }

    // ── Built-in converters: GUID ─────────────────────────────────────────────

    [Fact]
    public void BuiltInStringToGuidConverter_ConvertsCorrectly()
    {
        string guidStr = "d3b07384-d9a0-4b6e-8c1f-2a3b4c5d6e7f";
        ITypeConverter<string, Guid> converter = TypeConverters.StringToGuid;
        converter.Convert(guidStr).ShouldBe(Guid.Parse(guidStr));
    }

    // ── Built-in converters: DateTimeOffset ──────────────────────────────────

    [Fact]
    public void BuiltInStringToDateTimeOffsetConverter_ConvertsCorrectly()
    {
        string input = "2024-01-15T10:30:00+00:00";
        ITypeConverter<string, DateTimeOffset> converter = TypeConverters.StringToDateTimeOffset;
        converter.Convert(input).ShouldBe(DateTimeOffset.Parse(input, System.Globalization.CultureInfo.InvariantCulture));
    }

    // ── Built-in converters: enums ────────────────────────────────────────────

    [Fact]
    public void BuiltInStringToEnumConverter_ConvertsCorrectly()
    {
        ITypeConverter<string, DayOfWeek> converter = TypeConverters.StringToEnum<DayOfWeek>();
        converter.Convert("Monday").ShouldBe(DayOfWeek.Monday);
    }

    [Fact]
    public void BuiltInInt32ToEnumConverter_ConvertsCorrectly()
    {
        ITypeConverter<int, DayOfWeek> converter = TypeConverters.Int32ToEnum<DayOfWeek>();
        converter.Convert(1).ShouldBe(DayOfWeek.Monday);
    }

    // ── Profiles and helpers ─────────────────────────────────────────────────

    private sealed class OrderProfile : MappingProfile
    {
        protected override void Configure()
        {
            CreateMap<OrderSource, OrderTarget>();
        }
    }

    private sealed class OrderWithPerMapConverterProfile : MappingProfile
    {
        protected override void Configure()
        {
            CreateMap<OrderSource, OrderTarget>()
                .AddConverter(new StringToIntConverter());
        }
    }

    private sealed class StringToIntConverter : ITypeConverter<string, int>
    {
        public int Convert(string source) => int.Parse(source, System.Globalization.CultureInfo.InvariantCulture);
    }

    private sealed class OrderSource
    {
        public string? Quantity { get; set; }
    }

    private sealed class OrderTarget
    {
        public int Quantity { get; set; }
    }
}
