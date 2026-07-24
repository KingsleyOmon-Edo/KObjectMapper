namespace KObjectMapper;

public sealed class MappingProfileValidationException : InvalidOperationException
{
    public MappingProfileValidationException(IReadOnlyCollection<string> errors)
        : base(BuildMessage(errors))
    {
        ArgumentNullException.ThrowIfNull(errors);

        Errors = errors;
    }

    public IReadOnlyCollection<string> Errors { get; }

    private static string BuildMessage(IReadOnlyCollection<string> errors)
    {
        ArgumentNullException.ThrowIfNull(errors);

        if (errors.Count == 0)
        {
            throw new ArgumentException("At least one validation error is required.", nameof(errors));
        }

        return string.Join(Environment.NewLine, errors);
    }
}
