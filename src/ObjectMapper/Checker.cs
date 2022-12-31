namespace ObjectMapper;

public class Checker
{
    public static T NullChecks<T>(T source, T target)
    {
        source = source ?? throw new ArgumentNullException(nameof(source));
        target = target ?? throw new ArgumentNullException(nameof(target));
        return source;
    }
}