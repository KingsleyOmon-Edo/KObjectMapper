namespace KObjectMapper.Abstractions;

public sealed class MappingResult
{
    public bool IsSuccess { get; }
    public IReadOnlyList<MappingError> Errors { get; }

    private MappingResult(bool isSuccess, IReadOnlyList<MappingError> errors)
    {
        IsSuccess = isSuccess;
        Errors = errors;
    }

    public static MappingResult Success() => new(true, Array.Empty<MappingError>());
    public static MappingResult Failure(IReadOnlyList<MappingError> errors) => new(false, errors);
    public static MappingResult Failure(MappingError error) => new(false, new[] { error });
}
