namespace KObjectMapper.Abstractions;

/// <summary>
/// Represents the result of a safe enum conversion, carrying either the converted value or a structured error.
/// </summary>
/// <typeparam name="TEnum">The target enum type.</typeparam>
public sealed class EnumConversionResult<TEnum> where TEnum : struct, Enum
{
    private EnumConversionResult(TEnum value)
    {
        IsSuccess = true;
        Value = value;
        Error = null;
    }

    private EnumConversionResult(string error)
    {
        IsSuccess = false;
        Value = default;
        Error = error;
    }

    /// <summary>Gets a value indicating whether the conversion succeeded.</summary>
    public bool IsSuccess { get; }

    /// <summary>Gets the converted enum value when <see cref="IsSuccess"/> is true; otherwise the default value.</summary>
    public TEnum Value { get; }

    /// <summary>Gets the structured error message when <see cref="IsSuccess"/> is false; otherwise null.</summary>
    public string? Error { get; }

    internal static EnumConversionResult<TEnum> Success(TEnum value) => new(value);

    internal static EnumConversionResult<TEnum> Failure(string error) => new(error);
}
