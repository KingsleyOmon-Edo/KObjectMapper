using System.Diagnostics.CodeAnalysis;

namespace ObjectMapper;

public class Mapper
{
    private Mapper()
    {
    }

    public static Mapper Create()
    {
        return new Mapper();
    }

    public void Map<T>(T source, T target)
    {
        source = Checker.NullChecks<T>(source, target);

        source.ApplyDiffsTo<T>(target);
    }

    private static T NullChecks<T>(T source, T target)
    {
        source = source ?? throw new ArgumentNullException(nameof(source));
        target = target ?? throw new ArgumentNullException(nameof(target));
        return source;
    }


    public void Map(object source, object target)
    {
        Checker.NullChecks<object>(source, target);

        source.ApplyDiffsTo(target);
    }

    public void MapFrom<T>(T source, T target)
    {
        Checker.NullChecks<T>(source, target);

        target.ApplyDiffsTo<T>(source);
    }

    public void MapFrom(object source, object target)
    {
        Checker.NullChecks<object>(source, target);

        target.ApplyDiffsTo(source);
    }

    public void MapTo<T>(T source, T target)
    {
        Checker.NullChecks<T>(source, target);

        source.ApplyDiffsTo(target);
    }

    public void MapTo(object source, object target)
    {
        Checker.NullChecks<object>(source, target);

        target.ApplyDiffsTo(source);
    }

    public IEnumerable<T> MapFrom<T>(IEnumerable<T> source)
        where T : new()
    {
        var resultCollection = new List<T>();

        foreach (var sourceElem in source)
        {
            var targetElem = new T();
            sourceElem.MapTo(targetElem);

            resultCollection.Add(targetElem);
        }

        return resultCollection;
    }
}