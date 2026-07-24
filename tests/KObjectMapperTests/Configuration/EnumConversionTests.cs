using KObjectMapper.Abstractions;

namespace KObjectMapperTests.Configuration;

public class EnumConversionTests
{
    // ── Numeric-to-enum conversion ────────────────────────────────────────────

    [Theory]
    [InlineData(0, DayOfWeek.Sunday)]
    [InlineData(1, DayOfWeek.Monday)]
    [InlineData(6, DayOfWeek.Saturday)]
    public void EnumConverter_ConvertsInt32ToEnum_WhenValueIsDefined(int input, DayOfWeek expected)
    {
        IEnumConverter<int, DayOfWeek> converter = EnumConverter.FromInt32<DayOfWeek>();
        EnumConversionResult<DayOfWeek> result = converter.Convert(input);

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(expected);
    }

    [Fact]
    public void EnumConverter_ReturnsFailure_WhenInt32ValueIsNotDefinedInEnum()
    {
        IEnumConverter<int, DayOfWeek> converter = EnumConverter.FromInt32<DayOfWeek>();
        EnumConversionResult<DayOfWeek> result = converter.Convert(999);

        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBeNullOrWhiteSpace();
        result.Error.ShouldContain("999");
        result.Error.ShouldContain(nameof(DayOfWeek));
    }

    // ── String-to-enum conversion ─────────────────────────────────────────────

    [Theory]
    [InlineData("Sunday", DayOfWeek.Sunday)]
    [InlineData("Monday", DayOfWeek.Monday)]
    [InlineData("Saturday", DayOfWeek.Saturday)]
    public void EnumConverter_ConvertsStringToEnum_WhenValueIsValid(string input, DayOfWeek expected)
    {
        IEnumConverter<string, DayOfWeek> converter = EnumConverter.FromString<DayOfWeek>();
        EnumConversionResult<DayOfWeek> result = converter.Convert(input);

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(expected);
    }

    [Fact]
    public void EnumConverter_ReturnsFailure_WhenStringValueIsNotDefinedInEnum()
    {
        IEnumConverter<string, DayOfWeek> converter = EnumConverter.FromString<DayOfWeek>();
        EnumConversionResult<DayOfWeek> result = converter.Convert("NotADay");

        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBeNullOrWhiteSpace();
        result.Error.ShouldContain("NotADay");
        result.Error.ShouldContain(nameof(DayOfWeek));
    }

    // ── Case-insensitive string-to-enum parsing ───────────────────────────────

    [Theory]
    [InlineData("monday", DayOfWeek.Monday)]
    [InlineData("SATURDAY", DayOfWeek.Saturday)]
    [InlineData("SuNdAy", DayOfWeek.Sunday)]
    public void EnumConverter_ConvertsStringToEnum_CaseInsensitive_WhenOptionEnabled(string input, DayOfWeek expected)
    {
        IEnumConverter<string, DayOfWeek> converter = EnumConverter.FromString<DayOfWeek>(ignoreCase: true);
        EnumConversionResult<DayOfWeek> result = converter.Convert(input);

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(expected);
    }

    [Fact]
    public void EnumConverter_ReturnsFailure_WhenCaseMismatchAndCaseSensitive()
    {
        IEnumConverter<string, DayOfWeek> converter = EnumConverter.FromString<DayOfWeek>(ignoreCase: false);
        EnumConversionResult<DayOfWeek> result = converter.Convert("monday");

        result.IsSuccess.ShouldBeFalse();
    }

    // ── Deterministic failure details ─────────────────────────────────────────

    [Fact]
    public void EnumConversionResult_Failure_ContainsSourceValue_TargetType_AndReason()
    {
        IEnumConverter<string, DayOfWeek> converter = EnumConverter.FromString<DayOfWeek>();
        EnumConversionResult<DayOfWeek> result = converter.Convert("InvalidDay");

        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBeNull();
        result.Error!.ShouldContain("InvalidDay");
        result.Error!.ShouldContain(nameof(DayOfWeek));
    }

    [Fact]
    public void EnumConversionResult_Success_HasDefaultError()
    {
        IEnumConverter<string, DayOfWeek> converter = EnumConverter.FromString<DayOfWeek>();
        EnumConversionResult<DayOfWeek> result = converter.Convert("Monday");

        result.IsSuccess.ShouldBeTrue();
        result.Error.ShouldBeNull();
    }

    // ── ITypeConverter integration ────────────────────────────────────────────

    [Fact]
    public void EnumConverter_ImplementsITypeConverter_ForSuccessfulConversions()
    {
        ITypeConverter<string, DayOfWeek> converter = EnumConverter.FromString<DayOfWeek>().AsTypeConverter();
        DayOfWeek result = converter.Convert("Friday");
        result.ShouldBe(DayOfWeek.Friday);
    }

    [Fact]
    public void EnumConverter_AsTypeConverter_ThrowsMappingException_WhenConversionFails()
    {
        ITypeConverter<string, DayOfWeek> converter = EnumConverter.FromString<DayOfWeek>().AsTypeConverter();
        Should.Throw<InvalidOperationException>(() => converter.Convert("NotADay"));
    }
}
