namespace ObjectMapper;

public class Checker
{
    public static T NullChecks<T>(T source, T target)
    {
        source = source ?? throw new ArgumentNullException(nameof(source));
        target = target ?? throw new ArgumentNullException(nameof(target));
        return source;
    }

    public static void PropertyTypeCheck<T>(T source, T target)
    {
        if (source.GetType() != target.GetType())
        {
            throw new ArgumentException($"{nameof(source)} and {nameof(target)} objects should be of the same type");
        }
    }
}