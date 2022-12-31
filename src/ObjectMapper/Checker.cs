﻿namespace ObjectMapper;

public class Checker
{
    public static T NullChecks<T>(T source, T target)
    {
        source = CoalescedNullCheck<T>(source);
        target = CoalescedNullCheck<T>(target);

        return source;
    }

    public static T CoalescedNullCheck<T>(T source)
    {
        source = source ?? throw new ArgumentNullException(nameof(source));
        target = target ?? throw new ArgumentNullException(nameof(target));
        return source;
    }

    public static void PropertyTypeCheck<T>(T source, T target)
    {
        ;
        if (!AreSameType(source, target))
        {
            throw new ArgumentException($"{nameof(source)} and {nameof(target)} objects should be of the same type");
        }
    }

    private static bool AreSameType<T>(T source, T target)
    {
        return source?.GetType() == target?.GetType();
    }
}