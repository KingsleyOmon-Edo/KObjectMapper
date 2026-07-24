using System.Globalization;
using KObjectMapper.Abstractions;

namespace KObjectMapper.Converters;

/// <summary>
/// Provides built-in <see cref="ITypeConverter{TSource,TTarget}"/> instances for common type conversions.
/// </summary>
public static class TypeConverters
{
    /// <summary>Converts a <see cref="string"/> to <see cref="int"/>.</summary>
    public static ITypeConverter<string, int> StringToInt32 { get; } =
        new DelegateConverter<string, int>(s => int.Parse(s, CultureInfo.InvariantCulture));

    /// <summary>Converts a <see cref="string"/> to <see cref="long"/>.</summary>
    public static ITypeConverter<string, long> StringToInt64 { get; } =
        new DelegateConverter<string, long>(s => long.Parse(s, CultureInfo.InvariantCulture));

    /// <summary>Converts a <see cref="string"/> to <see cref="double"/>.</summary>
    public static ITypeConverter<string, double> StringToDouble { get; } =
        new DelegateConverter<string, double>(s => double.Parse(s, CultureInfo.InvariantCulture));

    /// <summary>Converts a <see cref="string"/> to <see cref="decimal"/>.</summary>
    public static ITypeConverter<string, decimal> StringToDecimal { get; } =
        new DelegateConverter<string, decimal>(s => decimal.Parse(s, CultureInfo.InvariantCulture));

    /// <summary>Converts a <see cref="string"/> to <see cref="bool"/>.</summary>
    public static ITypeConverter<string, bool> StringToBool { get; } =
        new DelegateConverter<string, bool>(s => bool.Parse(s));

    /// <summary>Converts a <see cref="string"/> to <see cref="Guid"/>.</summary>
    public static ITypeConverter<string, Guid> StringToGuid { get; } =
        new DelegateConverter<string, Guid>(Guid.Parse);

    /// <summary>Converts a <see cref="string"/> to <see cref="DateTimeOffset"/>.</summary>
    public static ITypeConverter<string, DateTimeOffset> StringToDateTimeOffset { get; } =
        new DelegateConverter<string, DateTimeOffset>(s =>
            DateTimeOffset.Parse(s, CultureInfo.InvariantCulture));

    /// <summary>Converts a <see cref="string"/> to <see cref="DateTime"/>.</summary>
    public static ITypeConverter<string, DateTime> StringToDateTime { get; } =
        new DelegateConverter<string, DateTime>(s =>
            DateTime.Parse(s, CultureInfo.InvariantCulture));

    /// <summary>Converts a <see cref="string"/> to the specified enum type using exact-match parsing.</summary>
    /// <typeparam name="TEnum">The target enum type.</typeparam>
    public static ITypeConverter<string, TEnum> StringToEnum<TEnum>() where TEnum : struct, Enum =>
        new DelegateConverter<string, TEnum>(s => Enum.Parse<TEnum>(s));

    /// <summary>Converts an <see cref="int"/> to the specified enum type.</summary>
    /// <typeparam name="TEnum">The target enum type.</typeparam>
    public static ITypeConverter<int, TEnum> Int32ToEnum<TEnum>() where TEnum : struct, Enum =>
        new DelegateConverter<int, TEnum>(i => (TEnum)Enum.ToObject(typeof(TEnum), i));

    private sealed class DelegateConverter<TSource, TTarget>(Func<TSource, TTarget> convert)
        : ITypeConverter<TSource, TTarget>
    {
        public TTarget Convert(TSource source) => convert(source);
    }
}
